using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.API.Configuration;
using MCH.API.ExternalServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCH.API.Controllers
{
    [Route("api/products")]
    public class ProductsController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;
        private readonly MlApi _mlApi;

        public ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger, IOptions<AppSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mlApi = new(settings.Value.MlApiUrl, _logger);
        }


        /// <summary>
        /// Получение списка товаров определенного производителя
        /// </summary>
        /// <param name="id">Id производителя</param>
        /// <param name="count">Максимальное количетсво товаров в ответе</param>
        /// <param name="offset">Спещение списка товаров</param>
        /// <returns></returns>
        [HttpGet]
        [Route("productsByCompany")]
        public async Task<ActionResult> GetProducts(int id, int count = 100, int offset = 0)
        {
            if (count <= 0)
            {
                return BadRequest("Количество запрашиваемых товаров должно быть положительным");
            }

            if (offset < 0)
            {
                return BadRequest("Смещение должно быть больше или равно нулю");
            }
            _logger.LogInformation($"Query to get products. CompanyId: {id}");
            return Ok(_unitOfWork.parsingRepository.GetProductsByCompany(id, count, offset));
        }

        /// <summary>
        /// Получение списка товаров без ограничений по производителю
        /// </summary>
        /// <param name="count">Максимальное количетсво товаров в ответе</param>
        /// <param name="offset">Спещение списка товаров</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> GetProducts(int count, int offset)
        {
            _logger.LogInformation($"Qery get top: {count} products with offset: {offset}");
            return Ok(_unitOfWork.parsingRepository.GetProducts(count, offset));
        }
        
        /// <summary>
        /// Получение списка товаров по строковому запросу (aka поиск)
        /// </summary>
        /// <param name="query">Поисковой запрос</param>
        /// <param name="count">Максимальное количество товаров в ответе</param>
        /// <returns></returns>
        [HttpGet]
        [Route("productsByQuery")]
        public async Task<ActionResult> GetProductsByQuery(string query, int count = 10)
        {
            _logger.LogInformation($"Query to get products. Query: {query}");
            var Ids = await _mlApi.getProductIdsByQuery(query, count);
            return Ok(_unitOfWork.parsingRepository.GetProductsByListId(Ids));
        }

        /// <summary>
        /// Получение количество товаров у производителя
        /// </summary>
        /// <param name="companyId">Id производителя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("countProducts")]
        public async Task<ActionResult> GetCountProducts(int companyId)
        {
            _logger.LogInformation($"Query to get count project by companyId: {companyId}");
            return Ok(_unitOfWork.parsingRepository.CountProducts(companyId));
        }

        /// <summary>
        /// Получение товара по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getById")]
        public async Task<ActionResult> GetProductById(int Id)
        {
            _logger.LogInformation($"Query to get product with id: {Id}");
            var product = _unitOfWork.parsingRepository.GetProductById(Id);
            if (product is not null)
            {
                return Ok(product);
            }

            return NotFound("Product not found");
        }

        
        /// <summary>
        /// Позвращает список похожих товаров
        /// </summary>
        /// <param name="productId">Id товара</param>
        /// <param name="count">Максимальное количество товаров в ответе</param>
        /// <returns></returns>
        [HttpGet]
        [Route("simularProducts")]
        public async Task<ActionResult> GetSimularProducts(int productId, int count)
        {
            _logger.LogInformation($"Query to get top: {count} similar product to product with id:{productId}");
            var Ids = await _mlApi.getSimularProducts(productId, count);
            return Ok(_unitOfWork.parsingRepository.GetProductsByListId(Ids));
        }
        
        
    }
}