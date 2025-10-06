using System.Text.Json.Serialization;

namespace ECommerce.Application.DTOs.Responses
{
    public class ItemResponse
    {
        [JsonPropertyName("produtoId")]
        public int ProductId { get; set; }

        [JsonPropertyName("descricao")]
        public string Description { get; set; }

        [JsonPropertyName("quantidade")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("precoUnitario")]
        public decimal UnitPrice { get; set; }
    }
}
