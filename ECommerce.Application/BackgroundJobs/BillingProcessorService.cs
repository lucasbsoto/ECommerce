using ECommerce.Application.Services.Abstractions;
using ECommerce.Domain._Core.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Infrastructure.ExternalModels.Billing;
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

                using (var scope = _serviceProvider.CreateScope())
                {
                    var billingClient = scope.ServiceProvider.GetRequiredService<IBillingHttpClient>();
                    var saleRepository = scope.ServiceProvider.GetRequiredService<IRepository<Sale>>();

                    var sale = await saleRepository.GetByIdAsync(saleId);
                    if (sale == null) continue;

                    try
                    {
                        var summary = new SaleSummaryDto
                        {
                            Identifier = sale.Identifier,
                            Subtotal = sale.TotalAmount, 
                            DiscountValue = sale.DiscountAmount,
                            TotalValue = sale.FinalAmount,
                            Items = sale.Items.Select(i => new ItemSummaryDto
                            {
                                Quantity = i.Quantity,
                                UnitPrice = i.UnitPrice
                            }).ToList()
                        };

                        await billingClient.SendToBillingAsync(summary);

                        sale.MarkAsDone();
                        await saleRepository.UpdateAsync(sale);
                    }
                    catch (Exception)
                    {
                        sale.MarkAsFailed();
                        await saleRepository.UpdateAsync(sale);
                    }
                }
            }
        }
    }
}