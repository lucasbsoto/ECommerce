namespace ECommerce.Domain.Rules.CustomerDiscount.Abstractions
{
    public interface IDiscountStrategy
    {
        decimal CalculateDiscount(decimal totalAmount);
    }
}
