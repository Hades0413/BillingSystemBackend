using BillingSystemBackend.Data;
using BillingSystemBackend.Models;

namespace BillingSystemBackend.Services;

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

            if (!success) return (false, mensaje, null);

            producto.ProductoId = productoId;
            return (true, "Producto registrado exitosamente.", producto);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al registrar el producto.", ex);
        }
    }

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

    public async Task<(bool success, string mensaje)> EditarProductoAsync(int productoId, Producto producto)
    {
        try
        {
            if (producto.ProductoImpuestoIgv == null)
                producto.ProductoImpuestoIgv = producto.ProductoPrecioVenta * 0.18m;

            var result = await _productoDbContext.EditarProductoAsync(
                productoId,
                producto.ProductoCodigo,
                producto.ProductoNombre,
                producto.ProductoStock,
                producto.ProductoPrecioVenta,
                producto.ProductoImpuestoIgv,
                producto.UnidadId,
                producto.CategoriaId,
                producto.ProductoImagen);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al editar el producto.", ex);
        }
    }

    public async Task<(bool success, string mensaje)> EliminarProductoAsync(int productoId)
    {
        try
        {
            var result = await _productoDbContext.EliminarProductoAsync(productoId);

            if (result > 0) return (true, "Producto eliminado exitosamente.");

            return (false, "No se pudo eliminar el producto.");
        }
        catch (Exception ex)
        {
            return (false, $"Error al eliminar el producto: {ex.Message}");
        }
    }
}