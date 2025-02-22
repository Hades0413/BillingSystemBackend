using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System;
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
            _rubroService = rubroService ?? throw new ArgumentNullException(nameof(rubroService), "El servicio de rubros no puede ser nulo.");
        }

        [HttpGet("listar")]
        public async Task<ActionResult<List<Rubro>>> ObtenerRubros()
        {
            try
            {
                var rubros = await _rubroService.ObtenerRubrosAsync();
                return Ok(rubros);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los rubros", error = ex.Message });
            }
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Rubro rubro)
        {
            if (rubro == null)
            {
                return BadRequest(new ErrorResponse("El rubro no puede ser nulo."));
            }

            if (string.IsNullOrEmpty(rubro.RubroNombre))
            {
                return BadRequest(new ErrorResponse("El nombre del rubro no puede estar vacío."));
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
