using System.Data;
using BillingSystemBackend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Data;

public class CategoriaDbContext : DbContext
{
    public CategoriaDbContext(DbContextOptions<CategoriaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }

    public async Task<List<Categoria>> ListarCategoriasConUsuarioIdAsync(int usuarioId)
    {
        if (usuarioId <= 0)
            throw new ArgumentException("El ID del usuario debe ser mayor que 0.", nameof(usuarioId));

        var parametros = new SqlParameter("@usuarioId", usuarioId);

        try
        {
            var categorias = await Categorias
                .FromSqlRaw("EXEC ListarCategoriasConUsuarioId @usuarioId", parametros)
                .AsNoTracking()
                .ToListAsync();

            if (categorias == null || categorias.Count == 0) return new List<Categoria>();

            return categorias;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ocurrió un error al obtener las categorías.", ex);
        }
    }

    public async Task<Categoria> ObtenerCategoriaPorIdAsync(int categoriaId)
    {
        if (categoriaId <= 0)
            throw new ArgumentException("El ID de la categoría debe ser mayor que 0.", nameof(categoriaId));

        return await Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoriaId == categoriaId);
    }

    public async Task<(bool success, int categoriaId, string mensaje)> RegistrarCategoriaAsync(int usuarioId,
        string categoriaNombre)
    {
        if (usuarioId <= 0 || string.IsNullOrWhiteSpace(categoriaNombre))
            throw new ArgumentException("El ID del usuario o el nombre de la categoría no pueden ser nulos.",
                nameof(usuarioId));

        var parameters = new[]
        {
            new SqlParameter("@usuario_id", usuarioId),
            new SqlParameter("@categoria_nombre", SqlDbType.NVarChar, 255) { Value = categoriaNombre },
            new SqlParameter("@categoria_id", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output }
        };

        using (var transaction = await Database.BeginTransactionAsync())
        {
            try
            {
                await Database.ExecuteSqlRawAsync(
                    "EXEC dbo.InsertarCategoria @usuario_id, @categoria_nombre, @categoria_id OUTPUT, @mensaje OUTPUT",
                    parameters);

                var categoriaId = (int)parameters[2].Value;
                var mensaje = parameters[3].Value.ToString();

                await transaction.CommitAsync();

                return categoriaId > 0
                    ? (true, categoriaId, mensaje)
                    : (false, categoriaId, mensaje);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Error al registrar la categoría. La transacción fue revertida.",
                    ex);
            }
        }
    }

    public async Task<bool> EditarCategoriaAsync(int categoriaId, string categoriaNombre)
    {
        if (categoriaId <= 0 || string.IsNullOrWhiteSpace(categoriaNombre))
            throw new ArgumentException("El ID de la categoría o el nombre no pueden ser nulos o vacíos.",
                nameof(categoriaId));

        var parameters = new[]
        {
            new SqlParameter("@categoria_id", categoriaId),
            new SqlParameter("@categoria_nombre", SqlDbType.NVarChar, 255) { Value = categoriaNombre },
            new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output }
        };

        try
        {
            await Database.ExecuteSqlRawAsync(
                "EXEC dbo.EditarCategoria @categoria_id, @categoria_nombre, @mensaje OUTPUT",
                parameters);

            var mensaje = parameters[2].Value.ToString();
            return !string.IsNullOrEmpty(mensaje);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al editar la categoría.", ex);
        }
    }

    public async Task<int> EliminarCategoriaAsync(int categoriaId)
    {
        var productosAsociados = await Productos
            .Where(p => p.CategoriaId == categoriaId)
            .AnyAsync();

        if (productosAsociados) return 0;

        var parametro = new SqlParameter("@categoria_id", categoriaId);

        try
        {
            var filasAfectadas = await Database.ExecuteSqlRawAsync("EXEC EliminarCategoria @categoria_id", parametro);

            return filasAfectadas;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al eliminar la categoría.", ex);
        }
    }
}