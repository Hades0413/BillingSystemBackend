using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BillingSystemBackend.Data
{
    public class ProductoDbContext : DbContext
    {
        public ProductoDbContext(DbContextOptions<ProductoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        
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
                    new SqlParameter("@producto_precio_venta", SqlDbType.Decimal) {
                        Value = productoPrecioVenta,
                        Precision = 18,
                        Scale = 2
                    },
                    new SqlParameter("@producto_impuesto_igv", SqlDbType.Decimal) {
                        Value = productoImpuestoIgv ?? (object)DBNull.Value,
                        Precision = 18,
                        Scale = 2
                    },
                    new SqlParameter("@unidad_id", unidadId),
                    new SqlParameter("@categoria_id", categoriaId),
                    new SqlParameter("@producto_imagen", SqlDbType.NVarChar, 500) { 
                        Value = string.IsNullOrEmpty(productoImagen) ? DBNull.Value : (object)productoImagen 
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
                        throw new InvalidOperationException("Error al registrar el producto, la transacción fue revertida.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar el producto con código {productoCodigo}: {ex.Message}", ex);
            }
        }
    }
}
