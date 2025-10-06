using ECommerce.Application.DTOs;
using FluentValidation;

namespace ECommerce.Application.Validations
{
    public class SaleRequestValidator : AbstractValidator<SaleRequest>
    {
        public SaleRequestValidator()
        {
            // Validação do Cliente
            RuleFor(x => x.Customer)
                .NotNull().WithMessage("Os dados do cliente são obrigatórios.")
                .SetValidator(new CustomerRequestValidator()); // Usa um validador aninhado

            // Validação da Data
            RuleFor(x => x.SaleDate)
                .GreaterThan(DateTime.Now.AddDays(-1)).WithMessage("A data da venda não pode ser muito antiga.");

            // Validação dos Itens
            RuleFor(x => x.Itens)
                .NotEmpty().WithMessage("A venda deve conter pelo menos um item.");

            RuleForEach(x => x.Itens).SetValidator(new ItemRequestValidator());
        }
    }
}