using ECommerce.Application.DTOs;
using FluentValidation;

namespace ECommerce.Application.Validations
{
    public class ItemRequestValidator : AbstractValidator<ItemRequest>
    {
        public ItemRequestValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A descrição do item é obrigatória.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("O preço unitário não pode ser negativo.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("O ID do produto deve ser válido.");
        }
    }
}
