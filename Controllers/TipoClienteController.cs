using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class TipoClienteController : ControllerBase
{
    private readonly TipoClienteService _tipoClienteService;

    public TipoClienteController(TipoClienteService tipoClienteService)
    {
        _tipoClienteService = tipoClienteService;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<TipoCliente>> RegistrarTipoCliente([FromBody] TipoCliente tipoCliente)
    {
        var nuevoTipoCliente = await _tipoClienteService.RegistrarTipoClienteAsync(tipoCliente);
        return CreatedAtAction(nameof(ListarTipoClientePorId), new { tipoClienteId = nuevoTipoCliente.TipoClienteId },
            nuevoTipoCliente);
    }

    [HttpGet("listar")]
    public async Task<ActionResult<List<TipoCliente>>> ListarTipoClientes()
    {
        var tipoClientes = await _tipoClienteService.ListarTipoClientesAsync();
        return Ok(tipoClientes);
    }

    [HttpGet("listar-por-id/{tipoClienteId}")]
    public async Task<ActionResult<TipoCliente>> ListarTipoClientePorId(int tipoClienteId)
    {
        var tipoCliente = await _tipoClienteService.ListarTipoClientePorIdAsync(tipoClienteId);
        if (tipoCliente == null) return NotFound();
        return Ok(tipoCliente);
    }

    [HttpGet("listar-por-nombre/{nombre}")]
    public async Task<ActionResult<List<TipoCliente>>> ListarTipoClientePorNombre(string nombre)
    {
        var tipoClientes = await _tipoClienteService.ListarTipoClientePorNombreAsync(nombre);
        if (tipoClientes == null || !tipoClientes.Any()) return NotFound();
        return Ok(tipoClientes);
    }
}