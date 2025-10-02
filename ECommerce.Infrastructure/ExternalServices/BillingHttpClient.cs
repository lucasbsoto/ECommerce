using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Infrastructure.ExternalServices.Abstractions;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization; // Usando o System.Text.Json nativo

namespace ECommerce.Infrastructure.ExternalServices
{
    // DTO que representa o JSON de Sumário do Anexo 2
    public class SaleSummaryDto
    {
        [JsonPropertyName("identificador")]
        public Guid Identifier { get; set; }

        [JsonPropertyName("subTotal")]
        public decimal Subtotal { get; set; } // Valor Total (sem desconto)

        [JsonPropertyName("descontos")]
        public decimal DiscountValue { get; set; }

        [JsonPropertyName("valorTotal")]
        public decimal TotalValue { get; set; } // Valor Final (com desconto)

        [JsonPropertyName("itens")]
        public List<ItemSummaryDto> Items { get; set; } = new List<ItemSummaryDto>();
    }

    public class ItemSummaryDto
    {
        [JsonPropertyName("quantidade")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("precoUnitario")]
        public decimal UnitPrice { get; set; }
    }

    public class BillingHttpClient : IBillingHttpClient
    {
        // O HttpClient deve ser gerenciado pelo IHttpClientFactory na DI do .NET 
        // para melhor gerenciamento de recursos e sockets.
        private readonly HttpClient _httpClient;
        private readonly string _billingApiUrl = "https://sti3-faturamento.azurewebsites.net/api/vendas"; // URL Fictícia

        // Em um projeto real, injetaríamos IHttpClientFactory e Logger
        public BillingHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendToBillingAsync(Sale sale)
        {
            // 1. Construir o JSON de Sumário (Anexo 2)
            var summary = new SaleSummaryDto
            {
                Identifier = sale.Identifier,
                Subtotal = sale.TotalAmount,     // Regra de Validação 1
                DiscountValue = sale.DiscountAmount,
                TotalValue = sale.FinalAmount,   // Regra de Validação 2
                Items = sale.Items.Select(i => new ItemSummaryDto
                {
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            // 2. Serializar para JSON
            var jsonContent = JsonSerializer.Serialize(summary);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("email", "teste@teste.com");

            // 3. Simular o envio
            // Aqui é onde você usaria Polly (se fosse um requisito explícito) 
            // para lidar com a indisponibilidade do serviço.

            Console.WriteLine($"[Billing] Enviando faturamento para Venda ID: {sale.Identifier}");

            // Simulação: o serviço de faturamento pode falhar.
            if (new Random().Next(1, 10) < 3)
            {
                Console.WriteLine($"[Billing] Erro SIMULADO no faturamento da Venda ID: {sale.Identifier}");
                throw new HttpRequestException("Serviço de Faturamento indisponível/falhou.");
            }

            // Simulação de chamada POST (com a URL configurada)
            var response = await _httpClient.PostAsync(_billingApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Faturamento falhou com código: {response.StatusCode}");
            }

            // Simular sucesso e atraso variável (requisito)
            await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 5)));

            Console.WriteLine($"[Billing] Faturamento da Venda ID {sale.Identifier} concluído com sucesso.");
        }
    }
}