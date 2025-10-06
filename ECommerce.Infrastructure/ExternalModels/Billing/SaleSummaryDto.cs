using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.ExternalModels.Billing
{
    public class SaleSummaryDto
    {
        [JsonPropertyName("identificador")]
        public Guid Identifier { get; set; }

        [JsonPropertyName("subTotal")]
        public decimal Subtotal { get; set; } // Valor Total (sem desconto)

        [JsonPropertyName("descontos")]
        public decimal DiscountValue { get; set; }

        [JsonPropertyName("valorTotal")]
        public decimal TotalValue { get; set; } // Valor Final (com desconto)

        [JsonPropertyName("itens")]
        public List<ItemSummaryDto> Items { get; set; } = new List<ItemSummaryDto>();
    }
}
