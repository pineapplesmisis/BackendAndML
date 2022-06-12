using System.Threading.Tasks;
using CheckersBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MCH.API.Controllers
{
    [Route("api/products")]
    public class ProductsController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

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
        
        [HttpGet]
        [Route("productsByQuery")]
        public async Task<ActionResult> GetProductsByQuery(string query, int count = 100, int offset = 0)
        {
            if (count <= 0)
            {
                return BadRequest("Количество запрашиваемых товаров должно быть положительным");
            }

            if (offset < 0)
            {
                return BadRequest("Смещение должно быть больше или равно нулю");
            }
            _logger.LogInformation($"Query to get products. Query: {query}");
            return Ok(_unitOfWork.parsingRepository.GetProductsbyQuery(query, count, offset));
        }

        [HttpGet]
        [Route("countProducts")]
        public async Task<ActionResult> GetCountProducts(int companyId)
        {
            _logger.LogInformation($"Query to get count project by companyId: {companyId}");
            return Ok(_unitOfWork.parsingRepository.CountProducts(companyId));
        }

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
    }
}