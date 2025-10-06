using ECommerce.Domain._Core;
using ECommerce.Infrastructure._Core;
using ECommerce.Infrastructure.ExternalModels.Billing;
using ECommerce.Infrastructure.ExternalServices.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ECommerce.Infrastructure.ExternalServices
{
    public class BillingHttpClient : IBillingHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private const string EmailHeaderKey = "Email";

        public BillingHttpClient(HttpClient httpClient, IOptions<BillingServiceOptions> options)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add(EmailHeaderKey, options.Value.Email);
            _endpoint = options.Value.Endpoint;
        }

        public async Task<Result> SendToBillingAsync(SaleSummaryDto saleSummary)
        {
            var jsonContent = JsonSerializer.Serialize(saleSummary);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            
            Console.WriteLine($"[Billing] Enviando faturamento para Venda ID: {saleSummary.Identifier}");

            if (new Random().Next(1, 10) < 3)
            {
                Console.WriteLine($"[Billing] Erro SIMULADO no faturamento da Venda ID: {saleSummary.Identifier}");
                return Result.Failure("Serviço de Faturamento indisponível/falhou.");
            }

            var response = await _httpClient.PostAsync(_endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure($"Faturamento falhou com código: {response.StatusCode}");
            }

            await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 5)));

            Console.WriteLine($"[Billing] Faturamento da Venda ID {saleSummary.Identifier} concluído com sucesso.");

            return Result.Success();
        }
    }
}