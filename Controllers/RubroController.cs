using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RubroController : ControllerBase
    {
        private readonly RubroService _rubroService;

        public RubroController(RubroService rubroService)
        {
            _rubroService = rubroService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerRubros()
        {
            try
            {
                var rubros = await _rubroService.ObtenerRubrosAsync();
                return Ok(new SuccessResponse("Rubros obtenidos correctamente.", rubros));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al obtener los rubros.", ex.Message));
            }
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Rubro rubro)
        {
            if (rubro == null || string.IsNullOrEmpty(rubro.RubroNombre))
            {
                return BadRequest(new ErrorResponse("El rubro no puede ser nulo o vacío."));
            }

            try
            {
                var (success, mensaje, rubroRegistrado) = await _rubroService.RegistrarRubroAsync(rubro.RubroNombre);

                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return Ok(new SuccessResponse(mensaje, rubroRegistrado));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al registrar el rubro.", ex.Message));
            }
        }
    }
}