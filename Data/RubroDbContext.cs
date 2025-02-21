using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic; // Para usar List<T>
using BillingSystemBackend.Models;

namespace BillingSystemBackend.Data
{
    public class RubroDbContext : DbContext
    {
        public RubroDbContext(DbContextOptions<RubroDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rubro> Rubros { get; set; }

        public async Task<List<Rubro>> ObtenerRubrosAsync()
        {
            try
            {
                return await Rubros.ToListAsync(); // Eliminamos redundancia de _context
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los rubros.", ex);
            }
        }

        public async Task<Rubro> ObtenerRubroPorIdAsync(int id)
        {
            try
            {
                return await Rubros.FirstOrDefaultAsync(r => r.RubroId == id); // Eliminamos redundancia de _context
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el rubro.", ex);
            }
        }

        public async Task<(bool success, int rubroId, string mensaje)> RegistrarRubroAsync(string rubroNombre)
        {
            try
            {
                var rubroIdParam = new SqlParameter("@rubroId", SqlDbType.Int) 
                { 
                    Direction = ParameterDirection.Output 
                };

                var mensajeParam = new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) 
                { 
                    Direction = ParameterDirection.Output 
                };

                await Database.ExecuteSqlRawAsync(
                    "EXEC dbo.InsertarRubro @rubro_nombre = {0}, @rubroId = @rubroId OUTPUT, @mensaje = @mensaje OUTPUT",
                    rubroNombre, rubroIdParam, mensajeParam);

                int rubroId = (int)rubroIdParam.Value;
                string mensaje = (string)mensajeParam.Value;

                return rubroId > 0 
                    ? (true, rubroId, mensaje) // Registro exitoso
                    : (false, rubroId, mensaje); // Error en el registro
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
            }
        }
    }
}
