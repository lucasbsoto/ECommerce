using ECommerce.Application.Models.Filters;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByFilters(CustomerFilter filter);
    }
}
