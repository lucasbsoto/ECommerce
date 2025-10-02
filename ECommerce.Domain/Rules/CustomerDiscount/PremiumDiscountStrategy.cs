using ECommerce.Domain.Rules.CustomerDiscount.Abstractions;

namespace ECommerce.Domain.Rules.CustomerDiscount
{
    public class PremiumDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal totalAmount)
        {
            return totalAmount > 300.00m ? Math.Round(totalAmount * 0.10m, 2, MidpointRounding.AwayFromZero) : 0m;
        }
    }
}
