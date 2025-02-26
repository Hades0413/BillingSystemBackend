using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class ClienteController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClienteController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<Cliente>> RegistrarCliente([FromBody] Cliente cliente)
    {
        var nuevoCliente = await _clienteService.RegistrarClienteAsync(cliente);
        return CreatedAtAction(nameof(ListarClientePorId), new { clienteId = nuevoCliente.ClienteId }, nuevoCliente);
    }

    [HttpGet("listar")]
    public async Task<ActionResult<List<Cliente>>> ListarClientes()
    {
        var clientes = await _clienteService.ListarClientesAsync();
        return Ok(clientes);
    }

    [HttpGet("listar-por-cliente/{clienteId}")]
    public async Task<ActionResult<Cliente>> ListarClientePorId(int clienteId)
    {
        var cliente = await _clienteService.ListarClientePorIdAsync(clienteId);
        if (cliente == null) return NotFound();
        return Ok(cliente);
    }

    [HttpGet("listar/nombre/{nombreLegal}")]
    public async Task<ActionResult<List<Cliente>>> ListarClientesPorNombreLegal(string nombreLegal)
    {
        var clientes = await _clienteService.ListarClientesPorNombreLegalAsync(nombreLegal);
        if (clientes == null || !clientes.Any()) return NotFound();
        return Ok(clientes);
    }
}