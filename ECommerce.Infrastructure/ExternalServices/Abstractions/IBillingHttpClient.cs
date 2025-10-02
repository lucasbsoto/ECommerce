using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.ExternalServices.Abstractions
{
    // Define o contrato de comunicação para o serviço de faturamento
    public interface IBillingHttpClient
    {
        // Recebe a venda para criar o sumário e enviá-lo.
        Task SendToBillingAsync(Sale sale);
    }
}