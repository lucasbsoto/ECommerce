using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost]
        public async Task<IActionResult> PostSale([FromBody] SaleRequest request)
        {
            if (request is null)
                return BadRequest("Venda inválida");

            var result = await _saleService.ProcessAndSaveSaleAsync(request);

            if (result.IsSuccess)
            {
                return Accepted(new
                {
                    Message = "Venda aceita e enviada para processamento assíncrono.",
                    Id = result.Value?.Identifier
                });
            }

            return BadRequest(new { result.Error });

        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SaleResponse>> GetSale(Guid id)
        {
            var sale = await _saleService.GetByIdSaleAsync(id);

            if (sale == null)
                return NotFound($"Venda {id} não encontrada.");
            

            return Ok(sale);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleResponse>>> GetAllSales()
        {
            var sales = await _saleService.GetAllSalesAsync();

            return Ok(sales);
        }

        [HttpPost("{id:guid}/retry-billing")]
        public async Task<IActionResult> RetryBilling(Guid id)
        {
            var result = await _saleService.RetryBillingAsync(id);

            if (result.IsFailure)
            {
                return NotFound(new { Error = result.Error });
            }
            return Accepted(new { Id = id, Message = "Retentativa de faturamento enfileirada com sucesso." });
        }
    }
}