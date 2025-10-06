using ECommerce.Application.Services.Abstractions;
using ECommerce.Domain._Core;
using System.Threading.Channels;

namespace ECommerce.Application.BackgroundJobs.Abstractions
{
    public class InMemoryBillingQueueService : IBillingQueueService
    {
        private readonly Channel<Guid> _queue;

        public InMemoryBillingQueueService()
        {
            _queue = Channel.CreateUnbounded<Guid>();
        }

        public Task<Result> Enqueue(Guid saleId)
        {
            if (saleId == Guid.Empty)
            {
                return Task.FromResult(Result.Failure($"O ID da venda {saleId}, não pode ser vazio."));
            }

            // Tenta escrever imediatamente. Como é Unbounded, isso deve ser síncrono.
            _queue.Writer.TryWrite(saleId);
            Console.WriteLine($"[Queue] Venda ID {saleId} enfileirada para faturamento.");

            return Task.FromResult(Result.Success());
        }

        public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
        {
            // Aguarda até que um item esteja disponível na fila
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
