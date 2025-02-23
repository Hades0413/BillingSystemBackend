using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _productoService;

        public ProductoController(ProductoService productoService)
        {
            _productoService = productoService ?? throw new ArgumentNullException(nameof(productoService));
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] Producto producto)
        {
            if (producto == null)
            {
                return BadRequest(new ErrorResponse("El producto no puede ser nulo."));
            }

            if (string.IsNullOrEmpty(producto.ProductoCodigo))
            {
                return BadRequest(new ErrorResponse("El código del producto no puede estar vacío."));
            }

            if (string.IsNullOrEmpty(producto.ProductoNombre))
            {
                return BadRequest(new ErrorResponse("El nombre del producto no puede estar vacío."));
            }

            try
            {
                var (success, mensaje, productoRegistrado) = await _productoService.RegistrarProductoAsync(producto);

                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return Ok(new SuccessResponse("Producto registrado exitosamente.", productoRegistrado));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Hubo un problema al registrar el producto. Intente nuevamente.", ex.Message));
            }
        }
        
        [HttpGet("listar/{usuarioId}")]
        public async Task<IActionResult> ListarProductos(int usuarioId)
        {
            if (usuarioId <= 0)
            {
                return BadRequest(new ErrorResponse("El ID del usuario debe ser mayor que 0."));
            }

            try
            {
                var productos = await _productoService.ListarProductosConUsuarioIdAsync(usuarioId);

                if (productos == null || productos.Count == 0)
                {
                    return NotFound(new ErrorResponse("No se encontraron productos para este usuario."));
                }

                return Ok(new SuccessResponse("Productos listados correctamente.", productos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Hubo un problema al listar los productos. Intente nuevamente.", ex.Message));
            }
        }
        
        
        [HttpPut("editar/{productoId}")]
        public async Task<IActionResult> Editar(int productoId, [FromBody] Producto producto)
        {
            if (productoId <= 0)
            {
                return BadRequest(new ErrorResponse("ID de producto inválido."));
            }

            if (producto == null)
            {
                return BadRequest(new ErrorResponse("El producto no puede ser nulo."));
            }

            try
            {
                var (success, mensaje) = await _productoService.EditarProductoAsync(productoId, producto);

                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return Ok(new SuccessResponse(mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Hubo un problema al editar el producto. Intente nuevamente.", ex.Message));
            }
        }
        
        [HttpDelete("eliminar/{productoId}")]
        public async Task<IActionResult> Eliminar(int productoId)
        {
            if (productoId <= 0)
            {
                return BadRequest(new ErrorResponse("ID de producto inválido."));
            }

            try
            {
                var (success, mensaje) = await _productoService.EliminarProductoAsync(productoId);

                if (!success)
                {
                    return BadRequest(new ErrorResponse(mensaje));
                }

                return Ok(new SuccessResponse(mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse("Hubo un problema al eliminar el producto. Intente nuevamente.", ex.Message));
            }
        }

    }
}
