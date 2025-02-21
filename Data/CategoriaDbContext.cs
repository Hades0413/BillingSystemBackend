using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic; // Para usar List<T>
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
            try
            {
                return await Categorias
                    .Where(c => c.UsuarioId == usuarioId) // Filtrar por usuarioId
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las categorías.", ex);
            }
        }

        public async Task<Categoria> ObtenerCategoriaPorIdAsync(int id)
        {
            try
            {
                return await Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la categoría.", ex);
            }
        }

        public async Task<(bool success, int categoriaId, string mensaje)> RegistrarCategoriaAsync(int usuarioId, string categoriaNombre)
        {
            try
            {
                // Definimos los parámetros de salida
                var categoriaIdParam = new SqlParameter("@categoriaId", SqlDbType.Int) 
                { 
                    Direction = ParameterDirection.Output 
                };

                var mensajeParam = new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) 
                { 
                    Direction = ParameterDirection.Output 
                };

                // Ejecutamos el procedimiento almacenado
                await Database.ExecuteSqlRawAsync(
                    "EXEC dbo.InsertarCategoria @categoria_nombre = {0}, @usuario_id = {1}, @categoriaId = @categoriaId OUTPUT, @mensaje = @mensaje OUTPUT",
                    categoriaNombre, usuarioId, categoriaIdParam, mensajeParam);

                // Obtenemos los valores de los parámetros de salida
                int categoriaId = (int)categoriaIdParam.Value;
                string mensaje = (string)mensajeParam.Value;

                // Devolvemos el resultado
                return categoriaId > 0 
                    ? (true, categoriaId, mensaje) // Registro exitoso
                    : (false, categoriaId, mensaje); // Error en el registro
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
            }
        }
    }
}
