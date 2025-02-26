using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class UnidadController : ControllerBase
{
    private readonly UnidadService _unidadService;

    public UnidadController(UnidadService unidadService)
    {
        _unidadService = unidadService ?? throw new ArgumentNullException(nameof(unidadService));
    }

    [HttpGet("listar")]
    public async Task<IActionResult> ListarUnidades()
    {
        try
        {
            var (unidades, mensaje, success) = await _unidadService.ListarUnidadesAsync();

            if (!success || unidades == null || unidades.Count == 0) return NotFound(new { mensaje });

            return Ok(new { mensaje, unidades });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener las unidades.", detalle = ex.Message });
        }
    }


    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] Unidad unidad)
    {
        if (unidad == null || string.IsNullOrEmpty(unidad.UnidadNombre))
            return BadRequest(new { mensaje = "El nombre de la unidad no puede ser nulo o vacío." });

        try
        {
            var (success, mensaje, unidadRegistrada) = await _unidadService.RegistrarUnidadAsync(unidad.UnidadNombre);
            if (!success) return BadRequest(new { mensaje });

            return CreatedAtAction(nameof(ListarUnidades), new { }, new { mensaje, unidad = unidadRegistrada });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al registrar la unidad.", detalle = ex.Message });
        }
    }
}