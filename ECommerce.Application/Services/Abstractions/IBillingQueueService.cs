namespace ECommerce.Application.Services.Abstractions
{
    public interface IBillingQueueService
    {
        void Enqueue(Guid saleId);
        Task<Guid> DequeueAsync(CancellationToken cancellationToken);
    }
}
