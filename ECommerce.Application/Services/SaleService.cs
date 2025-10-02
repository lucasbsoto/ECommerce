using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Abstractions;
using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Rules.CustomerDiscount;
using ECommerce.Domain.Rules.CustomerDiscount.Abstractions;

namespace ECommerce.Application.Services
{
    public class SaleService
    {
        private readonly IRepository<Sale> _saleRepository;
        // Interface do serviço de background (para a Fase 2)
        private readonly IBillingQueueService _billingQueue;

        public SaleService(IRepository<Sale> saleRepository, IBillingQueueService billingQueue)
        {
            _saleRepository = saleRepository;
            _billingQueue = billingQueue;
        }

        public async Task<Sale> ProcessAndSaveSaleAsync(SaleRequest request)
        {
            // 1. Mapear DTO para Entidade (simplificado, AutoMapper seria ideal)
            var sale = MapToSale(request);

            // 2. Selecionar a Estratégia de Desconto (o ponto chave do Strategy Pattern)
            IDiscountStrategy strategy = SelectDiscountStrategy(sale.Customer.Category);

            // 3. Processar Valores (Delega para a entidade)
            sale.ProcessValues(strategy);

            // 4. Persistir a Venda (Fase 1)
            await _saleRepository.AddAsync(sale);

            // 5. Enfileirar para Faturamento Assíncrono (Fase 2)
            _billingQueue.Enqueue(sale.Identifier);

            return sale;
        }

        private IDiscountStrategy SelectDiscountStrategy(CustomerCategory category)
        {
            return category switch
            {
                CustomerCategory.REGULAR => new RegularDiscountStrategy(),
                CustomerCategory.PREMIUM => new PremiumDiscountStrategy(),
                CustomerCategory.VIP => new VipDiscountStrategy(),
                _ => new NoDiscountStrategy(), // Implemente esta como 0m
            };
        }

        // Implementar o método MapToSale...
        private Sale MapToSale(SaleRequest request)
        {
            var customer = new Customer(request.Customer.CustomerId, 
                                        request.Customer.Name, 
                                        request.Customer.Cpf, 
                                        request.Customer.Category);

            var sale = new Sale(request.Identifier, 
                                request.SaleDate, 
                                request.Customer.CustomerId, 
                                customer,
                                request.Itens.Select(i => new SaleItem(i.ProductId, i.Description, i.Quantity, i.UnitPrice, request.Identifier))
                                             .ToList());

            return sale;
        }
    }
}