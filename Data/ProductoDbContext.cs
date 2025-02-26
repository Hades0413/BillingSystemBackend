using System.Data;
using BillingSystemBackend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Data;

public class ProductoDbContext : DbContext
{
    public ProductoDbContext(DbContextOptions<ProductoDbContext> options)
        : base(options)
    {
    }


    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Unidad> Unidades { get; set; }

    public async Task<List<Producto>> ListarProductosConUsuarioIdAsync(int usuarioId)
    {
        if (usuarioId <= 0)
            throw new ArgumentException("El ID del usuario debe ser mayor que 0.", nameof(usuarioId));

        var parametros = new SqlParameter("@UsuarioId", usuarioId);

        try
        {
            var productos = await Productos
                .FromSqlRaw("EXEC ListarProductosConUsuarioId @UsuarioId", parametros)
                .AsNoTracking()
                .ToListAsync();

            foreach (var producto in productos)
            {
                if (string.IsNullOrEmpty(producto.ProductoImagen)) producto.ProductoImagen = "default_image.png";

                if (!producto.ProductoImpuestoIgv.HasValue) producto.ProductoImpuestoIgv = 0m;
            }

            return productos;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ocurrió un error al obtener los productos.", ex);
        }
    }


    public async Task<(bool success, int productoId, string mensaje)> RegistrarProductoAsync(
        int usuarioId,
        string productoCodigo,
        string productoNombre,
        int productoStock,
        decimal productoPrecioVenta,
        decimal? productoImpuestoIgv,
        int unidadId,
        int categoriaId,
        string productoImagen)
    {
        if (usuarioId <= 0)
            throw new ArgumentException("El ID del usuario debe ser mayor que 0.");
        if (string.IsNullOrEmpty(productoCodigo))
            throw new ArgumentException("El código del producto no puede ser vacío.");
        if (string.IsNullOrEmpty(productoNombre))
            throw new ArgumentException("El nombre del producto no puede ser vacío.");
        if (productoPrecioVenta <= 0)
            throw new ArgumentException("El precio de venta debe ser mayor a 0.");
        if (productoStock < 0)
            throw new ArgumentException("El stock no puede ser negativo.");

        try
        {
            var parameters = new[]
            {
                new SqlParameter("@usuario_id", usuarioId),
                new SqlParameter("@producto_codigo", SqlDbType.NVarChar, 50) { Value = productoCodigo },
                new SqlParameter("@producto_nombre", SqlDbType.NVarChar, 255) { Value = productoNombre },
                new SqlParameter("@producto_stock", productoStock),
                new SqlParameter("@producto_precio_venta", SqlDbType.Decimal)
                {
                    Value = productoPrecioVenta,
                    Precision = 18,
                    Scale = 2
                },
                new SqlParameter("@producto_impuesto_igv", SqlDbType.Decimal)
                {
                    Value = productoImpuestoIgv ?? (object)DBNull.Value,
                    Precision = 18,
                    Scale = 2
                },
                new SqlParameter("@unidad_id", unidadId),
                new SqlParameter("@categoria_id", categoriaId),
                new SqlParameter("@producto_imagen", SqlDbType.NVarChar, 500)
                {
                    Value = string.IsNullOrEmpty(productoImagen) ? DBNull.Value : productoImagen
                },
                new SqlParameter("@producto_id", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output }
            };

            using (var transaction = await Database.BeginTransactionAsync())
            {
                try
                {
                    await Database.ExecuteSqlRawAsync(
                        "EXEC dbo.InsertarProducto " +
                        "@usuario_id, " +
                        "@producto_codigo, " +
                        "@producto_nombre, " +
                        "@producto_stock, " +
                        "@producto_precio_venta, " +
                        "@producto_impuesto_igv, " +
                        "@unidad_id, " +
                        "@categoria_id, " +
                        "@producto_imagen, " +
                        "@producto_id OUTPUT, " +
                        "@mensaje OUTPUT",
                        parameters);

                    var productoId = (int)parameters[9].Value;
                    var mensaje = parameters[10].Value.ToString();

                    await transaction.CommitAsync();

                    return productoId > 0
                        ? (true, productoId, mensaje)
                        : (false, productoId, mensaje);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("Error al registrar el producto, la transacción fue revertida.",
                        ex);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar el producto con código {productoCodigo}: {ex.Message}", ex);
        }
    }

    public async Task<(bool success, string mensaje)> EditarProductoAsync(
        int productoId,
        string productoCodigo,
        string productoNombre,
        int productoStock,
        decimal productoPrecioVenta,
        decimal? productoImpuestoIgv,
        int unidadId,
        int categoriaId,
        string productoImagen)
    {
        var parameters = new[]
        {
            new SqlParameter("@producto_id", SqlDbType.Int) { Value = productoId },
            new SqlParameter("@producto_codigo", SqlDbType.NVarChar, 50)
                { Value = productoCodigo ?? (object)DBNull.Value },
            new SqlParameter("@producto_nombre", SqlDbType.NVarChar, 255)
                { Value = productoNombre ?? (object)DBNull.Value },
            new SqlParameter("@producto_stock", SqlDbType.Int) { Value = productoStock },
            new SqlParameter("@producto_precio_venta", SqlDbType.Decimal) { Value = productoPrecioVenta },
            new SqlParameter("@producto_impuesto_igv", SqlDbType.Decimal)
                { Value = productoImpuestoIgv ?? (object)DBNull.Value },
            new SqlParameter("@unidad_id", SqlDbType.Int) { Value = unidadId },
            new SqlParameter("@categoria_id", SqlDbType.Int) { Value = categoriaId },
            new SqlParameter("@producto_imagen", SqlDbType.NVarChar, 500)
                { Value = string.IsNullOrEmpty(productoImagen) ? DBNull.Value : productoImagen }
        };

        try
        {
            var result = await Database.ExecuteSqlRawAsync(
                "EXEC dbo.EditarProducto @producto_id, @producto_codigo, @producto_nombre, @producto_stock, @producto_precio_venta, @producto_impuesto_igv, @unidad_id, @categoria_id, @producto_imagen",
                parameters);

            return (result > 0,
                result > 0 ? "Producto actualizado exitosamente." : "No se pudo actualizar el producto.");
        }
        catch (Exception ex)
        {
            return (false, $"Error al editar el producto: {ex.Message}");
        }
    }

    public async Task<int> EliminarProductoAsync(int productoId)
    {
        var parameters = new[]
        {
            new SqlParameter("@producto_id", SqlDbType.Int) { Value = productoId }
        };

        try
        {
            var result = await Database.ExecuteSqlRawAsync(
                "EXEC dbo.EliminarProducto @producto_id", parameters);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el producto.", ex);
        }
    }
}