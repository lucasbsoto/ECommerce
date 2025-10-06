using System.Text.Json.Serialization;

namespace ECommerce.Application.DTOs
{
    public class SaleRequest
    {
        [JsonPropertyName("identificador")]
        public Guid? Identifier { get; set; }

        [JsonPropertyName("dataVenda")]
        public DateTime? SaleDate { get; set; }

        [JsonPropertyName("cliente")]
        public CustomerRequest Customer { get; set; }

        [JsonPropertyName("itens")]
        public List<ItemRequest> Itens { get; set; } = new List<ItemRequest>();
    }
}
