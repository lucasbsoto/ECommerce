using ECommerce.Application.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ECommerce.Application.BackgroundJobs.Abstractions
{
    public class InMemoryBillingQueueService : IBillingQueueService
    {
        // Usando um Channel não limitado para enfileiramento assíncrono.
        // É thread-safe e suporta operações assíncronas (await).
        private readonly Channel<Guid> _queue;

        public InMemoryBillingQueueService()
        {
            // Channel.CreateUnbounded<T>() é uma boa escolha para filas simples
            _queue = Channel.CreateUnbounded<Guid>();
        }

        public void Enqueue(Guid saleId)
        {
            if (saleId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(saleId), "O ID da venda não pode ser vazio.");
            }

            // Tenta escrever imediatamente. Como é Unbounded, isso deve ser síncrono.
            _queue.Writer.TryWrite(saleId);
            Console.WriteLine($"[Queue] Venda ID {saleId} enfileirada para faturamento.");
        }

        public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
        {
            // Aguarda até que um item esteja disponível na fila
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
