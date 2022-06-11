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
        public async Task<ActionResult> GetProducts(int id, int count = 100)
        {
            _logger.LogDebug($"Query to get products. CompanyId: {id}");
            return Ok(_unitOfWork.parsingRepository.GetProductsByCompany(id, count));
        }
        
        [HttpGet]
        [Route("productsByQuery")]
        public async Task<ActionResult> GetProductsByQuery(string query, int count = 100)
        {
            _logger.LogDebug($"Query to get products. Query: {query}");
            return Ok(_unitOfWork.parsingRepository.GetProductsbyQuery(query, count));
        }
    }
}