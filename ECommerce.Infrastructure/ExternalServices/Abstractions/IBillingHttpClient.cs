using ECommerce.Domain._Core;
using ECommerce.Infrastructure.ExternalModels.Billing;

namespace ECommerce.Infrastructure.ExternalServices.Abstractions
{
    public interface IBillingHttpClient
    {
        Task<Result> SendToBillingAsync(SaleSummaryDto sale);
    }
}