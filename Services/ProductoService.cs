using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using System;
using System.Threading.Tasks;

namespace BillingSystemBackend.Services
{
    public class ProductoService
    {
        private readonly ProductoDbContext _productoDbContext;

        public ProductoService(ProductoDbContext productoDbContext)
        {
            _productoDbContext = productoDbContext;
        }

        public async Task<(bool success, string mensaje, Producto producto)> RegistrarProductoAsync(Producto producto)
        {
            try
            {
                var (success, productoId, mensaje) = await _productoDbContext.RegistrarProductoAsync(
                    producto.UsuarioId, 
                    producto.ProductoCodigo, 
                    producto.ProductoNombre, 
                    producto.ProductoStock, 
                    producto.ProductoPrecioVenta, 
                    producto.ProductoImpuestoIgv, 
                    producto.UnidadId, 
                    producto.CategoriaId, 
                    producto.ProductoImagen
                );

                if (!success)
                {
                    return (false, mensaje, null);
                }

                producto.ProductoId = productoId;
                return (true, "Producto registrado exitosamente.", producto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el producto.", ex);
            }
        }
        
        // Método para listar productos por ID de usuario
        public async Task<List<Producto>> ListarProductosConUsuarioIdAsync(int usuarioId)
        {
            try
            {
                return await _productoDbContext.ListarProductosConUsuarioIdAsync(usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos.", ex);
            }
        }
    }
}