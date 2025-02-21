using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly EmpresaService _empresaService;

        public EmpresaController(EmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Empresa empresa)
        {
            if (empresa == null)
            {
                return BadRequest(new ErrorResponse("La empresa no puede ser nula."));
            }

            try
            {
                var (empresaId, mensaje) = await _empresaService.RegistrarEmpresaAsync(empresa);
                if (empresaId > 0)
                {
                    return Ok(new SuccessResponse("Empresa registrada exitosamente.", empresa));
                }

                return BadRequest(new ErrorResponse(mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error interno del servidor.", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerEmpresa(int id)
        {
            var empresa = await _empresaService.ObtenerEmpresaPorIdAsync(id);
            if (empresa == null)
            {
                return NotFound(new ErrorResponse("Empresa no encontrada.", "No se encontró una empresa con este ID."));
            }
            return Ok(new SuccessResponse("Empresa encontrada.", empresa));
        }
    }
}