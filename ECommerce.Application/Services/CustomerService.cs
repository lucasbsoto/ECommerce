using ECommerce.Application.Models.Filters;
using ECommerce.Application.Services.Abstractions;
using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerService(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer?> GetCustomerByFilters(CustomerFilter filter)
        {
            var customer = _customerRepository.GetAll();

            if (filter.Identifier != Guid.Empty)
            {
                customer = customer.Where(x => x.CustomerId == filter.Identifier);
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                customer = customer.Where(x => x.Name == filter.Name);
            }

            if (!string.IsNullOrEmpty(filter.Cpf))
            {
                customer = customer.Where(x => x.Cpf == filter.Cpf);
            }

            if (Enum.IsDefined(typeof(CustomerCategory), filter.Category))
            {
                customer = customer.Where(x => x.Category == filter.Category);
            }

            return await customer.FirstOrDefaultAsync();
        }
    }
}