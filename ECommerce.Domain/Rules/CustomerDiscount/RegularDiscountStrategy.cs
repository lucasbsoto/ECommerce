using ECommerce.Domain.Rules.CustomerDiscount.Abstractions;

namespace ECommerce.Domain.Rules.CustomerDiscount
{
    public class RegularDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal totalAmount)
        {
            return totalAmount > 500.00m ? Math.Round(totalAmount * 0.05m, 2, MidpointRounding.AwayFromZero) : 0m;
        }
    }
}
