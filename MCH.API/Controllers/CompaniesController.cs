using System.Threading.Tasks;
using CheckersBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MCH.API.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/companies")]
    public class CompaniesController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;

        public CompaniesController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [Route("companies")]
        public async Task<ActionResult> GetAllCompanies()
        {
            return Ok(_unitOfWork.parsingRepository.getAllCompanies());
        }
    }
}