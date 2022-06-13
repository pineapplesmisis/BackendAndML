using System;
using System.Linq;
using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.API.Models;
using MCH.API.Validation;
using MCH.Parset.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MCH.API.Controllers
{
    [Route("api/companies")]
    public class CompaniesController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductsController> _logger;
        
        public CompaniesController(IUnitOfWork unitOfWork, ILogger<ProductsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        
        /// <summary>
        /// Получение списка всех производителей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("companies")]
        public async Task<ActionResult> GetAllCompanies()
        {
            try
            {
                return Ok(_unitOfWork.parsingRepository.getAllCompanies());
            }
            catch (Exception ex)
            {
                _logger.LogError("error while getting list of all companies");
                return BadRequest("Ошибка при получении списка производителей");
            }
        }

        /// <summary>
        /// Создание производителя
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
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
                await _unitOfWork.parsingRepository.AddCompanyAsync(companyEntity);
                await _unitOfWork.CommitAsync();
                return Ok(companyEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating new company. Message: {ex.Message}");
                return BadRequest("Ошибка во время сохранения производителя");
            }
        }

        /// <summary>
        /// Получение компании по ИНН
        /// </summary>
        /// <param name="INN">номер ИНН</param>
        /// <returns></returns>
        [HttpGet]
        [Route("companyByIIN")]
        public async Task<ActionResult> GetCompanyByIIN(string INN)
        {
            _logger.LogInformation($"Query to get company by IIN: {INN}");
            if (string.IsNullOrEmpty(INN) || !(INN.Length == 10 || INN.Length == 12) ||!INN.All(s => char.IsNumber(s)))
            {
                return BadRequest("IIN должен состоять из 10 или 12 чисел");
            }

            try
            {
                var company = _unitOfWork.parsingRepository.GetCompanyByIIN(INN);
                if (company is null)
                {
                    return NotFound("Произаодителя с таким ИИН нет");
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting company with INN: {INN}. Message: {ex.Message}");
                return BadRequest("Ошибка при получении протзводителя");
            }
        }
    }
}