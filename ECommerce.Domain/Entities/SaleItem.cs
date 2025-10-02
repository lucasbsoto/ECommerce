namespace ECommerce.Domain.Entities
{
    public class SaleItem
    {
        public SaleItem(int productId, string? description, decimal quantity, decimal unitPrice, Guid saleIdentifier)
        {
            ProductId = productId;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            SaleIdentifier = saleIdentifier;
        }
        public int ProductId { get; private set; }
        public string? Description { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public Guid SaleIdentifier { get; private set; }
    }
}
