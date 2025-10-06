namespace ECommerce.Infrastructure._Core
{
    public class BillingServiceOptions
    {
        public const string BillingService = "BillingService";

        public string BaseUrl { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
