using BillingSystemBackend.Services;
using BillingSystemBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [HttpGet("listar-categorias")]
        public async Task<IActionResult> ListarCategoriasConUsuarioId([FromQuery] int usuarioId)
        {

            if (usuarioId <= 0)
            {
                return BadRequest(new ErrorResponse("El ID del usuario es inválido."));
            }

            try
            {
                var categorias = await _categoriaService.ListarCategoriasConUsuarioIdAsync(usuarioId);

                if (categorias == null || categorias.Count == 0)
                {
                    return NotFound(new ErrorResponse("No se encontraron categorías para este usuario."));
                }

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
                return BadRequest(new ErrorResponse("La categoría o el nombre de la categoría no puede ser nulo o vacío."));
            }

            try
            {
                var (success, mensaje, categoriaRegistrada) = await _categoriaService.RegistrarCategoriaAsync(categoria.UsuarioId, categoria.CategoriaNombre);
                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return CreatedAtAction(nameof(ListarCategoriasConUsuarioId), new { usuarioId = categoria.UsuarioId }, new SuccessResponse(mensaje, categoriaRegistrada));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al registrar la categoría.", ex.Message));
            }
        }
    }
}
