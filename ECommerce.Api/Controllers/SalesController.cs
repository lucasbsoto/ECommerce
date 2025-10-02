using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly SaleService _saleService;

        public SalesController(SaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost]
        public async Task<IActionResult> PostSale([FromBody] SaleRequest request)
        {
            if (request is null)
                return BadRequest("Venda inválida");
            
            var sale = await _saleService.ProcessAndSaveSaleAsync(request);

            // Retorna 202 Accepted, indicando que a venda foi aceita e será processada
            return Accepted(new
            {
                Message = "Venda aceita e enviada para processamento assíncrono.",
                Id = sale.Identifier
            });
        }

        // Adicionar endpoints GET /sales/{id}, GET /sales, PUT /sales/{id} aqui (Diferenciais)
    }
}
