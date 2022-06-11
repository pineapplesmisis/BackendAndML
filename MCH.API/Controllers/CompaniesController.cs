using System;
using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.API.Models;
using MCH.API.Validation;
using MCH.Parset.Data.Entities;
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

        [HttpPost]
        [Route("createCompany")]
        public async Task<ActionResult> CreateCompany(Company company)
        {
            var validationResult = new CompanyValidation().Validate(company);
            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(validationResult.Errors.ToString(), ", "));
            }

            var companyEntity = new CompanyEntity()
            {
                CompanyName = company.Name,
                IIN = company.INN,
                Url = company.SiteUrl
            };
            try
            {
                await _unitOfWork.parsingRepository.CrateCompany(companyEntity);
                await _unitOfWork.CommitAsync();
                return Ok(companyEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating new company. Message: {ex.Message}");
                return BadRequest("Ошибка во время сохранения компании");
            }
        }
    }
}