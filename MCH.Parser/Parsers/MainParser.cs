using System;
using System.Collections.Generic;
using System.IO;
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

namespace MCH.Parsers
{
    public class MainParser: ParserBase
    {
        private readonly Dictionary<string, int> _checkedLinks;
        private readonly XmlRulesParser _xmlParser;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Company _company;
        private int currentRecursion = 0;
        public MainParser(IServiceScopeFactory serviceScopeFactory, string pathToXmlFiles)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _requests = new();
            _checkedLinks = new();
            _xmlParser = new(pathToXmlFiles);
        }

        public async Task Start(int companyId)
        {
            var rules = getRules(companyId);
            if (rules is null)
            {
                return;
            }
             _company = getCompany(companyId);
            var urlsToParse = getUrlsToParse(companyId);
            await ParseUrl(rules, urlsToParse.First());
        }

        private async  Task ParseUrl(ParseRules rules, UrlsToParse urlsToParse)
        {
            try
            {
                var url = new Uri(urlsToParse.Url);
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
                
            }
        }

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

            foreach (var productUrlrl in listProductsUrls)
            {
                var _url = productUrlrl;
                if (!productUrlrl.Contains(rules.UrlBase))
                {
                    _url = rules.UrlBase + _url;
                }

                if (!_checkedLinks.ContainsKey(_url))
                {
                    currentRecursion++;
                    if (currentRecursion > 20)
                    {
                        break;
                    }

                    try
                    {
                        await ParseListProducts(_url, rules);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    currentRecursion--;
                }
            }

        }

        private async Task ParseProduct(string url, ParseRules rules)
        {
            if (_checkedLinks.ContainsKey(url))
            {
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
                            if(int.TryParse(price.First().Attributes[rules.ProductPrice.TakenAttrubute]?.Value, out var priceInt))
                            {
                                product.Price = priceInt;
                            }
                        }
                        else
                        {
                            if(int.TryParse( price.First().InnerHtml, out var priceInt))
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



                if (!string.IsNullOrEmpty(product.ProductName))
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                        var productEntity = new ProductEntity()
                        {
                            CompanyId = _company.Id,
                            ProductName = TextCleaner.CleanString(product.ProductName),
                            Price = product.Price,
                            Description = TextCleaner.CleanString(product.Description),
                            Url = url
                        };
                        await unitOfWork.parsingRepository.SaveProductAsync(productEntity);
                        
                      var prodId =  await unitOfWork.CommitAsync();

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
                }
                
            }
            catch (Exception ex)
            {
                
            }
            //_logger.LogDebug(url);
        }

        private IEnumerable<string> getListProductUrls(string body, ParseRules rules)
        {
            return getUrls(rules.ListProductsUrl, body);
        }
        
        private IEnumerable<string> getProductUrls(string body, ParseRules rules)
        {
            return getUrls(rules.ProductUrl, body);
        }

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

        private ParseRules getRules(int companyId)
        {
            return _xmlParser.getRules(companyId);
            
        }

        private Company getCompany(int companyId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var compEmtity =  unitOfWork.parsingRepository.getCompanyEntity(companyId);
                return new()
                {
                    Id = compEmtity.Id,
                    CompanyName = compEmtity.CompanyName,
                    Url = compEmtity.Url,
                    IIN = compEmtity.IIN
                };

            }
        }

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

        private IUnitOfWork getUnitOfWork()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                return unitOfWork;
            }
        }
        
    }
}