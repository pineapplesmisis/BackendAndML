using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CheckersBackend.Data;
using HtmlAgilityPack;
using MCH.Data.Entities;
using MCH.Models;
using MCH.Utils.Products;
using MCH.XmlRules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MCH.Parsers
{
    public class MainParser: ParserBase
    {
        private readonly Dictionary<string, int> _checkedLinks;
        private readonly XmlRulesParser _xmlParser;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private int _companyId;
        private int currentRecursion = 0;
        private readonly ILogger _logger;
        public MainParser(IServiceScopeFactory serviceScopeFactory, string pathToXmlFiles, ILogger logger)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _requests = new();
            _checkedLinks = new();
            _xmlParser = new(pathToXmlFiles);
        }

        public async Task Start(int companyId)
        {
            _companyId = companyId;
            var rules = _xmlParser.getRules(companyId);
            if (rules is null)
            {
                _logger.LogWarning($"Xml file for company: {companyId} not parsed.");
                return;
            }
            
            var urlsToParse = getUrlsToParse(companyId);
            await ParseUrl(rules, urlsToParse.First());
        }


    /// <summary>
    /// Выполняет парсинг сайта, начиная с указанной ссылки
    /// </summary>
    /// <param name="rules">Правила парсинга</param>
    /// <param name="urlsToParse">ссылка</param>
        private async  Task ParseUrl(ParseRules rules, UrlsToParse urlsToParse)
        {
            try
            {
                var isListProduct = IsListProducts(rules, urlsToParse.Url);
                if (isListProduct)
                {
                    await ParseListProducts(urlsToParse.Url, rules);
                }
                else
                {
                    var isProduct = IsProduct(rules, urlsToParse.Url);
                    if (isProduct)
                    {
                        ParseListProducts(urlsToParse.Url, rules);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while parsing url: {urlsToParse}");
            }
        }

        /// <summary>
        /// Поиск ссылок и переход по ним на странице со списком продуктов
        /// </summary>
        /// <param name="url">Ссылка</param>
        /// <param name="rules">Правила парсинга</param>
        private async  Task ParseListProducts( string url, ParseRules rules)
        {
            if (_checkedLinks.ContainsKey(url))
            {
                return;
            }
            _checkedLinks[url] = 1;
            var body = await _requests.CreateRequest(new Uri(url));
            
            var productUrls = getProductUrls(body, rules);

            foreach (var productUrl in productUrls)
            {
                var _url = productUrl;
                if (!_url.Contains(rules.UrlBase))
                {
                    _url = rules.UrlBase + _url;
                }
                await ParseProduct(_url, rules);
            }
            var listProductsUrls = getListProductUrls(body, rules);

            foreach (var productUrl in listProductsUrls)
            {
                var _url = productUrl;
                if (!productUrl.Contains(rules.UrlBase))
                {
                    _url = rules.UrlBase + _url;
                }
                
                await ParseListProducts(_url, rules);
            }

        }

        /// <summary>
        /// Парсит ссылку с продуктом
        /// </summary>
        /// <param name="url">ссылка</param>
        /// <param name="rules">правила парсинга</param>
        private async Task ParseProduct(string url, ParseRules rules)
        {
            if (_checkedLinks.ContainsKey(url))
            {
                //Выход, если уже просмотрели эту ссылку
                return;
            }
            _checkedLinks[url] = 1;
            try
            {
                var body = await _requests.CreateRequest(new Uri(url));
                var product = new Product();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(body);
                if (rules.ProductName is not null)
                {
                    var name = doc.DocumentNode.SelectNodes(rules.ProductName.expression);
                    if (name != null && name.Any())
                    {
                        if (!string.IsNullOrEmpty(rules.ProductName.TakenAttrubute))
                        {
                            product.ProductName = name.First().Attributes[rules.ProductImage.TakenAttrubute].Value;
                        }
                        else
                        {
                            product.ProductName = name.First().InnerHtml;
                        }
                    }
                    
                }

                if (rules.ProductPrice is not null)
                {
                    var price = doc.DocumentNode.SelectNodes(rules.ProductPrice.expression);
                    if (price != null && price.Any())
                    {
                        if (!string.IsNullOrEmpty(rules.ProductPrice.TakenAttrubute))
                        {
                            if(int.TryParse(TextCleaner.CleanNumber(price.First().Attributes[rules.ProductPrice.TakenAttrubute]?.Value), out var priceInt))
                            {
                                product.Price = priceInt;
                            }
                        }
                        else
                        {
                            if(int.TryParse( TextCleaner.CleanNumber(price.First().InnerHtml), out var priceInt))
                            {
                                product.Price = priceInt;
                            };
                        }
                    }
                    
                }

                if (rules.ProductDescription is not null)
                {
                    var desc = doc.DocumentNode.SelectNodes(rules.ProductDescription.expression);
                    if (desc != null && desc.Any())
                    {
                        if (!string.IsNullOrEmpty(rules.ProductDescription.TakenAttrubute))
                        {
                            product.Description = desc.First().Attributes[rules.ProductImage.TakenAttrubute]?.Value;
                        }
                        else
                        {
                            product.Description = desc.First().InnerHtml;
                        }
                    }
                }

                if (rules.ProductImage is not null)
                {
                    var img = doc.DocumentNode.SelectNodes(rules.ProductImage.expression);
                    if (img != null && img.Any())
                    {
                        if (!string.IsNullOrEmpty(rules.ProductImage.TakenAttrubute))
                        {
                            var imageUrl1 = img.First();
                            var imageUrl = img.First().Attributes[rules.ProductImage.TakenAttrubute]?.Value;
                            if (!imageUrl.Contains(rules.UrlBase))
                            {
                                imageUrl = rules.UrlBase + imageUrl;
                            }

                            product.Image = imageUrl;
                        }
                        else
                        {
                            product.Image = img.First().InnerHtml;
                        }
                    }
                }

                await SaveProduct(product, url);


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while parsing product. Url: {url}. Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохранение продукта в базу данных
        /// </summary>
        /// <param name="product">Продукт</param>
        /// <param name="url">Ссылка</param>
        private async Task SaveProduct(Product product, string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(product.ProductName))
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                        var productEntity = new ProductEntity()
                        {
                            CompanyId = _companyId,
                            ProductName = TextCleaner.CleanString(product.ProductName),
                            Price = product.Price,
                            Description = TextCleaner.CleanString(product.Description),
                            Url = url
                        };
                        await unitOfWork.parsingRepository.SaveProductAsync(productEntity);

                        var prodId = await unitOfWork.CommitAsync();

                        if (product.Image is not null)
                        {
                            await unitOfWork.parsingRepository.AddImageAsync(new()
                            {
                                Url = product.Image,
                                ProductId = productEntity.Id
                            });
                            await unitOfWork.CommitAsync();
                        }
                    }
                    _logger.LogInformation($"Product with name: {product.ProductName} saved in db. Url: {url}");
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while saving product with name: {product.ProductName}. Url: {url}. Message: {ex.Message}");
            }
        }

        
        /// <summary>
        /// Получение ссылок на страницы с набором продуктов
        /// </summary>
        /// <param name="body">html body</param>
        /// <param name="rules">Правила среза</param>
        /// <returns></returns>
        private IEnumerable<string> getListProductUrls(string body, ParseRules rules)
        {
            return getUrls(rules.ListProductsUrl, body);
        }
        
        /// <summary>
        /// Получение ссылок на страницы с продуктом
        /// </summary>
        /// <param name="body">html body</param>
        /// <param name="rules">Правила среза</param>
        /// <returns></returns>
        private IEnumerable<string> getProductUrls(string body, ParseRules rules)
        {
            return getUrls(rules.ProductUrl, body);
        }

        
        
        /// <summary>
        /// Получениее ссылок из body(html),
        /// которые соответствуют regex-выражениям
        /// </summary>
        /// <param name="regexes">regex-выражения</param>
        /// <param name="body">html body</param>
        /// <returns></returns>
        private IEnumerable<string> getUrls(IEnumerable<Regex> regexes, string body)
        {
            var urls = new List<string>();
            foreach (var regxListProd in regexes)
            {
                var matches = regxListProd.Matches(body);
                urls.AddRange(matches.Select(x => x.Value.Trim()));
            }

            return urls.ToList().Distinct();

        }

        
        /// <summary>
        /// Проверка ялвяется ли ссылка ссылкой
        /// на страницу с набором продуктов
        /// </summary>
        /// <param name="rules">Правила парсинга</param>
        /// <param name="Url">Ссылка</param>
        /// <returns></returns>
        private bool IsListProducts(ParseRules rules, string Url)
        {
            foreach (var regxListProd in rules.ListProductsUrl)
            {
                var match = regxListProd.Match(Url);
                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }
        
        
        /// <summary>
        /// Проверка - является ли ссылка ссылкой
        /// на страницу с продуктом
        /// </summary>
        /// <param name="rules">Правила парсинга</param>
        /// <param name="Url">Ссылка</param>
        /// <returns></returns>
        private bool IsProduct(ParseRules rules, string Url)
        {
            foreach (var regxListProd in rules.ProductUrl)
            {
                var match = regxListProd.Match(Url);
                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }

        
        /// <summary>
        /// получение ссылок для старта парсинга
        /// </summary>
        /// <param name="companyId">Id компании, сайт которой будет парситься</param>
        /// <returns></returns>
        private IEnumerable<UrlsToParse> getUrlsToParse(int companyId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                var urlsToParseEntities = unitOfWork.parsingRepository.getUrlsToParse(companyId);
                return urlsToParseEntities
                    .Select(x => new UrlsToParse()
                    {
                        Url = x.Url,
                        Id = x.Id,
                        CompanyId = x.CompanyId
                    }).ToList<UrlsToParse>();
            }
        }

    }
}