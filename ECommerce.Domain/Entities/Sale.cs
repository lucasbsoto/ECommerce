using ECommerce.Domain.Enums;
using ECommerce.Domain.Rules.CustomerDiscount.Abstractions;

namespace ECommerce.Domain.Entities
{
    public class Sale
    {
        public Sale()
        {
            Items = new List<SaleItem>();
        }

        public Sale(Guid? identifier, DateTime? saleDate, Guid customerId)
        {
            Identifier = identifier ?? Guid.NewGuid();
            SaleDate = saleDate ?? DateTime.Now;
            CustomerId = customerId;
            Status = SaleStatus.PENDING;
        }

        public Guid Identifier { get; private set; }
        public DateTime SaleDate { get; private set; }
        public Guid CustomerId { get; private set; }
        public SaleStatus Status { get; private set; }
        public DateTime? BillingDate { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal FinalAmount { get; private set; }
        public Customer Customer { get; set; }
        public List<SaleItem> Items { get; set; }

        public void MarkAsDone()
        {
            Status = SaleStatus.DONE;
            BillingDate = DateTime.Now;
        }

        public void MarkAsFailed()
        {
            Status = SaleStatus.FAILED;
        }

        /// <summary>
        /// Método que DELEGA o cálculo para a Estratégia injetada pelo Service
        /// </summary>
        /// <param name="strategy"></param>
        public void ProcessValues(IDiscountStrategy strategy)
        {
            decimal subtotal = Items.Sum(i => i.Quantity * i.UnitPrice);

            TotalAmount = Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);

            DiscountAmount = strategy.CalculateDiscount(TotalAmount);

            FinalAmount = TotalAmount - DiscountAmount;
        }
    }
}
