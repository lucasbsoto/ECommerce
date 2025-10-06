using ECommerce.Domain.Enums;
using System.Text.Json.Serialization;

namespace ECommerce.Application.DTOs.Responses
{
    public class CustomerResponse
    {
        public Guid CustomerId { get; set; }

        public string Name { get; set; }

        public string Cpf { get; set; }

        public CustomerCategory Category { get; set; }
    }
}
