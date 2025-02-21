using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BillingSystemBackend.Models;

namespace BillingSystemBackend.Data
{
    public class CategoriaDbContext : DbContext
    {
        public CategoriaDbContext(DbContextOptions<CategoriaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }

        public async Task<List<Categoria>> ObtenerCategoriasAsync(int usuarioId)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("El ID del usuario debe ser mayor que 0.");

            return await Categorias
                .Where(c => c.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Categoria> ObtenerCategoriaPorIdAsync(int categoriaId)
        {
            if (categoriaId <= 0)
                throw new ArgumentException("El ID de la categoría debe ser mayor que 0.");

            return await Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoriaId == categoriaId);
        }

        public async Task<(bool success, int categoriaId, string mensaje)> RegistrarCategoriaAsync(int usuarioId, string categoriaNombre)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("El ID del usuario debe ser mayor que 0.");

            if (string.IsNullOrWhiteSpace(categoriaNombre))
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.");

            try
            {
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
                        throw new InvalidOperationException("Error al registrar la categoría. La transacción fue revertida.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar la categoría {categoriaNombre} para el usuario con ID {usuarioId}: {ex.Message}", ex);
            }
        }
    }
}
