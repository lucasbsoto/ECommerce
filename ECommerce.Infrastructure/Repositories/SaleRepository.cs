using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class SaleRepository : IRepository<Sale>
    {
        private readonly AppDbContext _context;

        public SaleRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(Sale entity)
        {
            await _context.Sales.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sale entity)
        {
            _context.Sales.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Sale?> GetByIdAsync(Guid id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Identifier == id);
        }

        public IQueryable<Sale> GetAll()
        {
            return _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Items)
                .AsNoTracking();
        }
    }
}