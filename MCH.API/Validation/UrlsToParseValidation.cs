using CheckersBackend.Data;
using FluentValidation;
using MCH.API.Models;

namespace MCH.API.Validation
{
    public class UrlsToParseValidation: AbstractValidator<UrlsToParse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UrlsToParseValidation(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(u => u.CompanyId)
                .Must(IsCompanyExist)
                .WithMessage($"Компании с таким Id нет");

            RuleFor(u => u.Url)
                .NotEmpty()
                .WithMessage("Ссылка не может быть пустой");
        }

        private  bool IsCompanyExist(int companyId)
        {
            var company = _unitOfWork.parsingRepository.getCompanyEntity(companyId);
            return company is not null;
        }
    }
    
}