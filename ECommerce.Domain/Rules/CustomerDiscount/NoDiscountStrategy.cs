using ECommerce.Domain.Rules.CustomerDiscount.Abstractions;

namespace ECommerce.Domain.Rules.CustomerDiscount
{
    public class NoDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal totalAmount)
        {
            return 0m;
        }
    }
}
