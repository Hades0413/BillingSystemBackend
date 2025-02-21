using Microsoft.AspNetCore.Mvc;
using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using System.Threading.Tasks;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            if (usuario == null)
            {
                return BadRequest(new ErrorResponse("El usuario no puede ser nulo."));
            }

            try
            {
                var result = await _usuarioService.RegistrarUsuarioAsync(usuario);
                if (result)
                {
                    return Ok(new SuccessResponse("Usuario registrado exitosamente.", usuario));
                }

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

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuario(int id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                return NotFound(new ErrorResponse("Usuario no encontrado.", "No se encontró un usuario con este ID."));
            }
            return Ok(new SuccessResponse("Usuario encontrado.", usuario));
        }
    }
}
