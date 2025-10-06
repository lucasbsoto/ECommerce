using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain._Core;

namespace ECommerce.Application.Services.Abstractions
{
    public interface ISaleService
    {
        Task<Result<SaleResponse>> ProcessAndSaveSaleAsync(SaleRequest request);

        Task<SaleResponse?> GetByIdSaleAsync(Guid id);

        Task<IEnumerable<SaleResponse>> GetAllSalesAsync();

        Task<Result> RetryBillingAsync(Guid Id);
    }
}
