using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.ExternalModels.Billing
{
    public class ItemSummaryDto
    {
        [JsonPropertyName("quantidade")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("precoUnitario")]
        public decimal UnitPrice { get; set; }
    }
}
