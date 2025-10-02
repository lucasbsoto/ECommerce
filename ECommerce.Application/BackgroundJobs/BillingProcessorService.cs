using ECommerce.Application.Services.Abstractions;
using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Infrastructure.ExternalServices.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ECommerce.Application.BackgroundJobs
{
    // Usa o BackgroundService nativo do .NET
    public class BillingProcessorService : BackgroundService
    {
        private readonly IBillingQueueService _queue;
        private readonly IServiceProvider _serviceProvider; // Para criar escopos

        public BillingProcessorService(IBillingQueueService queue, IServiceProvider serviceProvider)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var saleId = await _queue.DequeueAsync(stoppingToken);

                // IMPORTANTE: Criar um escopo para DbContext e Scoped Services (como Repositories)
                using (var scope = _serviceProvider.CreateScope())
                {
                    var billingClient = scope.ServiceProvider.GetRequiredService<IBillingHttpClient>();
                    var saleRepository = scope.ServiceProvider.GetRequiredService<IRepository<Sale>>();

                    var sale = await saleRepository.GetByIdAsync(saleId);
                    if (sale == null) continue;

                    try
                    {
                        // 1. Enviar requisição (simulando chamada externa)
                        await billingClient.SendToBillingAsync(sale);

                        // 2. Atualizar status para CONCLUIDO
                        sale.MarkAsDone();
                        await saleRepository.UpdateAsync(sale);
                    }
                    catch (Exception)
                    {
                        // Lidar com indisponibilidade (logar, tentar novamente mais tarde, etc.)
                        sale.MarkAsFailed();
                        await saleRepository.UpdateAsync(sale);
                        // Opcional: Re-enfileirar com delay
                    }
                }
            }
        }
    }
}