using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoComprobanteController : ControllerBase
    {
        private readonly TipoComprobanteService _tipoComprobanteService;

        public TipoComprobanteController(TipoComprobanteService tipoComprobanteService)
        {
            _tipoComprobanteService = tipoComprobanteService;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> ListarTipoComprobantes()
        {
            var resultado = await _tipoComprobanteService.ObtenerTipoComprobantesAsync();

            if (!resultado.Success)
            {
                return BadRequest(new { message = resultado.Message });
            }

            return Ok(new { message = resultado.Message, data = resultado.Data });
        }
    }
}