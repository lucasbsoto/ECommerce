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

        public Sale(Guid? identifier, DateTime? saleDate, Guid customerId, Customer customer, List<SaleItem> items)
        {
            Identifier = identifier.HasValue ? identifier.Value : Guid.NewGuid();
            SaleDate = saleDate ?? DateTime.Now;
            CustomerId = customerId;
            Customer = customer;
            Items = items ?? [];
            Status = SaleStatus.PENDING;
        }

        public Guid Identifier { get; private set; }
        public DateTime SaleDate { get; private set; }
        public Guid CustomerId { get; private set; }
        public SaleStatus Status { get; private set; }


        // Propriedades de Navegação (EF Core)

        public Customer Customer { get; set; }
        public List<SaleItem> Items { get;  set; }

        // Campos persistíveis no DB
        public decimal DiscountAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal FinalAmount { get; private set; }

        public void MarkAsDone()
        {
            Status = SaleStatus.DONE;
        }

        public void MarkAsFailed()
        {
            Status = SaleStatus.FAILED;
        }

        // Método que DELEGA o cálculo para a Estratégia injetada pelo Service
        public void ProcessValues(IDiscountStrategy strategy)
        {
            // 1. Calcula o Valor Bruto
            decimal subtotal = Items.Sum(i => i.Quantity * i.UnitPrice);

            // 2. Aplica o arredondamento (requisito do desafio)
            TotalAmount = Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);

            // 3. Calcula o desconto usando o Padrão Estratégia
            DiscountAmount = strategy.CalculateDiscount(TotalAmount);

            // 4. Calcula o valor final
            FinalAmount = TotalAmount - DiscountAmount;
        }
    }
}
