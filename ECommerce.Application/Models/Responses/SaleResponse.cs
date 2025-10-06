using ECommerce.Domain.Enums;
using System.Text.Json.Serialization;

namespace ECommerce.Application.DTOs.Responses
{
    public class SaleResponse
    {
        public Guid Identifier { get; set; }
        public DateTime SaleDate { get; set; }
        public CustomerResponse Customer { get; set; }
        public List<ItemResponse> Itens { get; set; } = new List<ItemResponse>();
        public SaleStatus Status { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
