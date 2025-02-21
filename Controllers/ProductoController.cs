using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _productoService;

        public ProductoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Producto producto)
        {
            if (producto == null || string.IsNullOrEmpty(producto.ProductoCodigo) || string.IsNullOrEmpty(producto.ProductoNombre))
            {
                return BadRequest(new ErrorResponse("El producto no puede ser nulo o vacío."));
            }

            try
            {
                var (success, mensaje, productoRegistrado) = await _productoService.RegistrarProductoAsync(producto);

                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return Ok(new SuccessResponse(mensaje, productoRegistrado));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Error al registrar el producto.", ex.Message));
            }
        }
    }
}