using FluentValidation;
using MCH.API.Models;

namespace MCH.API.Validation
{
    public class CompanyValidation: AbstractValidator<Company>
    {
        public CompanyValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Название компании не может быть пустым");

            RuleFor(x => x.INN)
                .NotEmpty()
                .Length(10, 12)
                .WithMessage("ИНН должно состоять из 10 или 12 цифр");

            RuleFor(x => x.SiteUrl)
                .NotEmpty()
                .WithMessage("Ссылка на сайт компании не может быть пустой");

        }
    }
}