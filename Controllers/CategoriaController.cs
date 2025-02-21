using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaService _categoriaService;

        public CategoriaController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias([FromQuery] int usuarioId)
        {
            try
            {
                var categorias = await _categoriaService.ObtenerCategoriasAsync(usuarioId);
                return Ok(new SuccessResponse("Categorías obtenidas correctamente.", categorias));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al obtener las categorías.", ex.Message));
            }
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Categoria categoria)
        {
            if (categoria == null || string.IsNullOrEmpty(categoria.CategoriaNombre))
            {
                return BadRequest(new ErrorResponse("La categoría no puede ser nula o vacía."));
            }

            try
            {
                var (success, mensaje, categoriaRegistrada) = await _categoriaService.RegistrarCategoriaAsync(categoria.UsuarioId, categoria.CategoriaNombre);

                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return Ok(new SuccessResponse(mensaje, categoriaRegistrada));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al registrar la categoría.", ex.Message));
            }
        }
    }
}