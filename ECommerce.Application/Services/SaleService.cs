using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Application.Models.Filters;
using ECommerce.Application.Services.Abstractions;
using ECommerce.Domain._Core;
using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Rules.CustomerDiscount;
using ECommerce.Domain.Rules.CustomerDiscount.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly IRepository<Sale> _saleRepository;
        private readonly IBillingQueueService _billingQueue;
        private readonly ICustomerService _customerService;
        private readonly IValidator<SaleRequest> _saleValidator;

        public SaleService(IRepository<Sale> saleRepository, IBillingQueueService billingQueue, ICustomerService customerService, IValidator<SaleRequest> saleValidator)
        {
            _saleRepository = saleRepository;
            _saleValidator = saleValidator;
            _billingQueue = billingQueue;
            _customerService = customerService;
        }

        public async Task<Result<SaleResponse>> ProcessAndSaveSaleAsync(SaleRequest request)
        {
            var validationResult = await _saleValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<SaleResponse>.Failure($"Falha na validação dos dados de entrada: {errors}");
            }

            var filter = new CustomerFilter(request.Customer.CustomerId, request.Customer.Name, request.Customer.Cpf, request.Customer.Category);
            var customer = await _customerService.GetCustomerByFilters(filter);
            var sale = customer is not null ? request.MapToSale(customer) : request.MapToSale();

            try
            {
                IDiscountStrategy strategy = SelectDiscountStrategy(sale.Customer.Category);
                sale.ProcessValues(strategy);

                await _saleRepository.AddAsync(sale);
                await _billingQueue.Enqueue(sale.Identifier);

                return Result<SaleResponse>.Success(sale.MapToResponse());
            }
            catch (Exception ex)
            {
                return Result<SaleResponse>.Failure($"Erro ao processar a venda: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<SaleResponse?> GetByIdSaleAsync(Guid id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);

            return sale == null ? null : sale.MapToResponse();
        }

        public async Task<IEnumerable<SaleResponse>> GetAllSalesAsync()
        {
            var sales = _saleRepository.GetAll();

            return await sales.Select(x => x.MapToResponse()).ToListAsync();
        }

        public async Task<Result> RetryBillingAsync(Guid id)
        {
            await _billingQueue.Enqueue(id);

            return Result.Success();
        }

        private IDiscountStrategy SelectDiscountStrategy(CustomerCategory category)
        {
            return category switch
            {
                CustomerCategory.REGULAR => new RegularDiscountStrategy(),
                CustomerCategory.PREMIUM => new PremiumDiscountStrategy(),
                CustomerCategory.VIP => new VipDiscountStrategy(),
                _ => new NoDiscountStrategy(),
            };
        }
    }
}