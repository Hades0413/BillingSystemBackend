using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
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
            return await Categorias
                .Where(c => c.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Categoria> ObtenerCategoriaPorIdAsync(int id)
        {
            return await Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoriaId == id);
        }

        public async Task<(bool success, int categoriaId, string mensaje)> RegistrarCategoriaAsync(int usuarioId, string categoriaNombre)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@usuario_id", usuarioId),
                    new SqlParameter("@categoria_nombre", SqlDbType.NVarChar, 255) { Value = categoriaNombre },
                    new SqlParameter("@categoria_id", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output }
                };

                await Database.ExecuteSqlRawAsync(
                    "EXEC dbo.InsertarCategoria @usuario_id, @categoria_nombre, @categoria_id OUTPUT, @mensaje OUTPUT",
                    parameters);

                var categoriaId = (int)parameters[2].Value;
                var mensaje = parameters[3].Value.ToString();

                return categoriaId > 0 
                    ? (true, categoriaId, mensaje)
                    : (false, categoriaId, mensaje);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar categoría: {ex.Message}", ex);
            }
        }
    }
}