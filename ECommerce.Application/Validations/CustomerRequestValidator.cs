using ECommerce.Application.DTOs;
using FluentValidation;

namespace ECommerce.Application.Validations
{
    public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
    {
        public CustomerRequestValidator()
        {
            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("O CPF do cliente é obrigatório.")
                .Length(14).WithMessage("O CPF deve ter 14 dígitos.")
                .Must(BeValidCpf).WithMessage("O CPF informado é inválido.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do cliente é obrigatório.");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("A categoria do cliente é inválida.");
        }

        private bool BeValidCpf(string cpf)
        {
            return !string.IsNullOrWhiteSpace(cpf);// && cpf.All(char.IsDigit);
        }
    }
}