using ECommerce.Domain.Rules.CustomerDiscount.Abstractions;

namespace ECommerce.Domain.Rules.CustomerDiscount
{
    public class VipDiscountStrategy : IDiscountStrategy
    {
        public decimal CalculateDiscount(decimal totalAmount)
        {
            return Math.Round(totalAmount * 0.15m, 2, MidpointRounding.AwayFromZero);
        }
    }
}
