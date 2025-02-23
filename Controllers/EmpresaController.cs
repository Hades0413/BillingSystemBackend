using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

        [HttpGet("listar/{usuarioId}")]
        public async Task<IActionResult> ListarEmpresasPorUsuarioId(int usuarioId)
        {
            if (usuarioId <= 0)
            {
                return BadRequest(new ErrorResponse("El ID del usuario debe ser mayor a cero."));
            }

            try
            {
                var empresas = await _empresaService.ListarEmpresasPorUsuarioIdAsync(usuarioId);
                if (empresas == null || !empresas.Any())
                {
                    return NotFound(new ErrorResponse("No se encontraron empresas para este usuario."));
                }

                return Ok(new SuccessResponse("Empresas encontradas.", empresas));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al listar las empresas.", ex.Message));
            }
        }
        [HttpGet("listar-por-ruc/{ruc}")]
        public async Task<IActionResult> ListarPorRuc(string ruc)
        {
            if (string.IsNullOrWhiteSpace(ruc))
            {
                return BadRequest(new ErrorResponse("El RUC es obligatorio."));
            }

            try
            {
                var empresas = await _empresaService.ListarEmpresasPorRucAsync(ruc);
                if (empresas == null || empresas.Count == 0)
                {
                    return NotFound(new ErrorResponse("No se encontraron empresas con el RUC proporcionado."));
                }

                return Ok(empresas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error interno del servidor al listar empresas por RUC.", ex.Message));
            }
        }

        

        
    }
}
