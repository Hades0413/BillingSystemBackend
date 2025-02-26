using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuarioController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
    {
        if (usuario == null) return BadRequest(new ErrorResponse("El usuario no puede ser nulo."));

        try
        {
            var result = await _usuarioService.RegistrarUsuarioAsync(usuario);
            if (result) return Ok(new SuccessResponse("Usuario registrado exitosamente.", usuario));

            return BadRequest(new ErrorResponse("Ya existe un usuario con este correo."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ErrorResponse(ex.Message, "Verifique los datos del formulario."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("Error interno del servidor.", ex.Message));
        }
    }

    [HttpGet("listar/{id}")]
    public async Task<IActionResult> ObtenerUsuario(int id)
    {
        var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

        if (usuario == null)
            return NotFound(new ErrorResponse("Usuario no encontrado.", "No se encontró un usuario con este ID."));
        return Ok(new SuccessResponse("Usuario encontrado.", usuario));
    }

    [HttpGet("correo/{correo}")]
    public async Task<IActionResult> ObtenerUsuarioConCorreo(string correo)
    {
        var usuario = await _usuarioService.ObtenerUsuarioConCorreoAsync(correo);

        if (usuario == null)
            return NotFound(new ErrorResponse("Usuario no encontrado.", "No se encontró un usuario con este correo."));
        return Ok(new SuccessResponse("Usuario encontrado.", usuario));
    }
    
    [HttpGet("listar")]
    public async Task<IActionResult> ObtenerTodosUsuarios()
    {
        var usuarios = await _usuarioService.ObtenerTodosUsuariosAsync();

        if (usuarios == null || usuarios.Count == 0)
            return NotFound(new ErrorResponse("No se encontraron usuarios.", "La lista de usuarios está vacía."));

        return Ok(new SuccessResponse("Usuarios encontrados.", usuarios));
    }

    [HttpPut("editar/{id}")]
    public async Task<IActionResult> EditarUsuario(int id, [FromBody] Usuario usuario)
    {
        if (usuario == null) return BadRequest(new ErrorResponse("El usuario no puede ser nulo."));

        try
        {
            var usuarioExistente = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
            if (usuarioExistente == null)
                return NotFound(new ErrorResponse("Usuario no encontrado.", "No se encontró un usuario con este ID."));

            usuario.UsuarioId = id;
            usuario.UsuarioFechaUltimaActualizacion = DateTime.Now; 
            
            var result = await _usuarioService.ActualizarUsuarioAsync(usuario);
            if (result)
                return Ok(new SuccessResponse("Usuario actualizado exitosamente.", usuario));

            return BadRequest(new ErrorResponse("No se pudo actualizar el usuario. Intente nuevamente."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("Error interno del servidor.", ex.Message));
        }
    }

    
}