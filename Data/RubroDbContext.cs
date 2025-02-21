using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
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
                return await Rubros.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los rubros.", ex);
            }
        }

        public async Task<Rubro> ObtenerRubroPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del rubro debe ser mayor a cero.", nameof(id));

            try
            {
                return await Rubros.AsNoTracking().FirstOrDefaultAsync(r => r.RubroId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el rubro con ID {id}.", ex);
            }
        }

        public async Task<(bool success, int rubroId, string mensaje)> RegistrarRubroAsync(string rubroNombre)
        {
            if (string.IsNullOrEmpty(rubroNombre))
                throw new ArgumentException("El nombre del rubro no puede ser vacío.", nameof(rubroNombre));

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

                using (var transaction = await Database.BeginTransactionAsync())
                {
                    try
                    {
                        await Database.ExecuteSqlRawAsync(
                            "EXEC dbo.InsertarRubro @rubro_nombre = {0}, @rubroId = @rubroId OUTPUT, @mensaje = @mensaje OUTPUT",
                            rubroNombre, rubroIdParam, mensajeParam);

                        int rubroId = (int)rubroIdParam.Value;
                        string mensaje = (string)mensajeParam.Value;

                        await transaction.CommitAsync();

                        return rubroId > 0
                            ? (true, rubroId, mensaje)
                            : (false, rubroId, mensaje);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new InvalidOperationException("Error al registrar el rubro, la transacción fue revertida.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar el procedimiento almacenado para registrar el rubro.", ex);
            }
        }
    }
}
