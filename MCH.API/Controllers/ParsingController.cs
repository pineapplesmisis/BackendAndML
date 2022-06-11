using System;
using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.API.Models;
using MCH.API.Validation;
using MCH.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MCH.API.Controllers
{
    [Route("api/parsing")]
    public class ParsingController: Controller
    {
        private readonly ILogger<ParsingController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ParsingController(ILogger<ParsingController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        [Route("createUrlToParse")]
        public async Task<ActionResult> createUrlToParse([FromBody] UrlsToParse urlsToParse)
        {
            var validationresult = new UrlsToParseValidation(_unitOfWork).Validate(urlsToParse);

            if (!validationresult.IsValid)
            {
                return BadRequest(string.Join(validationresult.Errors.ToString(), ", "));
            }

            var ulrToParseEntity = new UrlsToParseEntity()
            {
                CompanyId = urlsToParse.CompanyId,
                Url = urlsToParse.Url
            };

            try
            {

                await _unitOfWork.parsingRepository.CrateUrlToParseAsync(ulrToParseEntity);
                await _unitOfWork.CommitAsync();
                return Ok(ulrToParseEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating new UrlsToParse. Message: {ex.Message}");
                return BadRequest("Ошибка по время сохранения ссылки в базу");
            }
            
        }
    }
}