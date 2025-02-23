using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using BillingSystemBackend.Models;

namespace BillingSystemBackend.Data
{
    public class UnidadDbContext : DbContext
    {
        public UnidadDbContext(DbContextOptions<UnidadDbContext> options)
            : base(options)
        {
        }

        public DbSet<Unidad> Unidades { get; set; }

        public async Task<(List<Unidad> unidades, string mensaje, bool success)> ListarUnidadesAsync()
        {
            var parameters = new[]
            {
                new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output },
                new SqlParameter("@estado", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            try
            {
                var unidades = await Unidades
                    .FromSqlRaw("EXEC dbo.ListarUnidades @mensaje OUTPUT, @estado OUTPUT", parameters)
                    .AsNoTracking()
                    .ToListAsync();

                var mensaje = parameters[0].Value.ToString();
                var estado = (int)parameters[1].Value;

                return estado == 1
                    ? (unidades, mensaje, true)
                    : (null, mensaje, false);
            }
            catch (Exception ex)
            {
                return (null, "Error al obtener las unidades.", false);
            }
        }


        public async Task<(bool success, int unidadId, string mensaje)> RegistrarUnidadAsync(string unidadNombre)
        {
            if (string.IsNullOrWhiteSpace(unidadNombre))
                throw new ArgumentException("El nombre de la unidad no puede ser nulo o vacío.", nameof(unidadNombre));

            var parameters = new[]
            {
                new SqlParameter("@unidad_nombre", SqlDbType.NVarChar, 255) { Value = unidadNombre },
                new SqlParameter("@unidad_id", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@mensaje", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output },
                new SqlParameter("@estado", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            try
            {
                await Database.ExecuteSqlRawAsync(
                    "EXEC dbo.InsertarUnidad @unidad_nombre, @unidad_id OUTPUT, @mensaje OUTPUT, @estado OUTPUT",
                    parameters);

                var unidadId = (int)parameters[1].Value;
                var mensaje = parameters[2].Value.ToString();
                var estado = (int)parameters[3].Value;

                return estado == 1
                    ? (true, unidadId, mensaje)
                    : (false, unidadId, mensaje);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al registrar la unidad.", ex);
            }
        }

    }
}
