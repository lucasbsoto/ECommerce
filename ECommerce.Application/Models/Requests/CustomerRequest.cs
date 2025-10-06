using ECommerce.Domain.Enums;
using System.Text.Json.Serialization;

namespace ECommerce.Application.DTOs
{
    public class CustomerRequest
    {
        [JsonPropertyName("clienteId")]
        public Guid? CustomerId { get; set; }

        [JsonPropertyName("nome")]
        public string Name { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("categoria")]
        [JsonConverter(typeof(JsonStringEnumConverter))]

        public CustomerCategory Category { get; set; }
    }
}
