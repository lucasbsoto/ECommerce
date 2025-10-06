using ECommerce.Domain._Core;

namespace ECommerce.Application.Services.Abstractions
{
    public interface IBillingQueueService
    {
        Task<Result> Enqueue(Guid saleId);
        Task<Guid> DequeueAsync(CancellationToken cancellationToken);
    }
}
