namespace ECommerce.Domain._Core.Abstractions
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T?> GetByIdAsync(Guid id);
        IQueryable<T> GetAll();
        Task UpdateAsync(T entity); // Necessário para alterar o status na Fase 2
    }
}
