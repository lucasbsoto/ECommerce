using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers
{
    public static class SaleMapper
    {
        public static Sale MapToSale(this SaleRequest request, Customer? customer = null)
        {
            customer ??= new Customer(request.Customer.CustomerId ?? Guid.NewGuid(),
                                        request.Customer.Name,
                                        request.Customer.Cpf,
                                        request.Customer.Category);

            var sale = new Sale(request.Identifier,
                                request.SaleDate,
                                customer.CustomerId)
            {
                Customer = customer
            };
            sale.Items = request.Itens.Select(i => new SaleItem(i.ProductId, i.Description, i.Quantity, i.UnitPrice, sale.Identifier))
                                      .ToList() ?? [];

            return sale;
        }

        public static SaleResponse MapToResponse(this Sale sale)
        {
            var saleResponse = new SaleResponse
            {
                Identifier = sale.Identifier,
                SaleDate = sale.SaleDate,
                Customer = new CustomerResponse
                {
                    CustomerId = sale.Customer.CustomerId,
                    Name = sale.Customer.Name,
                    Cpf = sale.Customer.Cpf,
                    Category = sale.Customer.Category
                },
                Itens = sale.Items.Select(i => new ItemResponse
                {
                    ProductId = i.ProductId,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                Status = sale.Status,
                DiscountAmount = sale.DiscountAmount,
                TotalAmount = sale.TotalAmount,
                FinalAmount = sale.FinalAmount
            };

            return saleResponse;
        }
    }
}
