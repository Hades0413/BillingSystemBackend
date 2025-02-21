using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly EmpresaService _empresaService;

        public EmpresaController(EmpresaService empresaService)
        {
            _empresaService = empresaService ?? throw new ArgumentNullException(nameof(empresaService));
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Empresa empresa)
        {
            if (empresa == null)
            {
                return BadRequest(new ErrorResponse("La empresa no puede ser nula."));
            }

            if (string.IsNullOrWhiteSpace(empresa.EmpresaRazonSocial))
            {
                return BadRequest(new ErrorResponse("Razón Social es obligatorios."));
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
                return StatusCode(500, new ErrorResponse("Error interno del servidor al registrar la empresa.", ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerEmpresa(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ErrorResponse("El ID de la empresa debe ser mayor a cero."));
            }

            try
            {
                var empresa = await _empresaService.ObtenerEmpresaPorIdAsync(id);
                if (empresa == null)
                {
                    return NotFound(new ErrorResponse("Empresa no encontrada.", "No se encontró una empresa con este ID."));
                }

                return Ok(new SuccessResponse("Empresa encontrada.", empresa));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error interno al obtener la empresa.", ex.Message));
            }
        }
    }
}
