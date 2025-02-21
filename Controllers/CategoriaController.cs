using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaService _categoriaService;

        public CategoriaController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService ?? throw new ArgumentNullException(nameof(categoriaService));
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias([FromQuery] int usuarioId)
        {
            if (usuarioId <= 0)
            {
                return BadRequest(new ErrorResponse("El ID del usuario es inválido."));
            }

            try
            {
                var categorias = await _categoriaService.ObtenerCategoriasAsync(usuarioId);
                if (categorias == null || categorias.Count == 0)
                {
                    return NotFound(new ErrorResponse("No se encontraron categorías para este usuario."));
                }

                return Ok(new SuccessResponse("Categorías obtenidas correctamente.", categorias));
            }
            catch (Exception ex)
            {
                // Se puede especificar un tipo de excepción más concreto si es necesario
                return StatusCode(500, new ErrorResponse("Error al obtener las categorías.", ex.Message));
            }
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Categoria categoria)
        {
            if (categoria == null)
            {
                return BadRequest(new ErrorResponse("La categoría no puede ser nula."));
            }

            if (string.IsNullOrEmpty(categoria.CategoriaNombre))
            {
                return BadRequest(new ErrorResponse("El nombre de la categoría no puede estar vacío."));
            }

            try
            {
                var (success, mensaje, categoriaRegistrada) = await _categoriaService.RegistrarCategoriaAsync(categoria.UsuarioId, categoria.CategoriaNombre);

                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return CreatedAtAction(nameof(ObtenerCategorias), new { usuarioId = categoria.UsuarioId }, new SuccessResponse(mensaje, categoriaRegistrada));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al registrar la categoría.", ex.Message));
            }
        }
    }
}
