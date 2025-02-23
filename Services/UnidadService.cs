using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystemBackend.Services
{
    public class UnidadService
    {
        private readonly UnidadDbContext _unidadDbContext;

        public UnidadService(UnidadDbContext unidadDbContext)
        {
            _unidadDbContext = unidadDbContext;
        }

        public async Task<(List<Unidad> unidades, string mensaje, bool success)> ListarUnidadesAsync()
        {
            try
            {
                var (unidades, mensaje, success) = await _unidadDbContext.ListarUnidadesAsync();

                if (!success)
                {
                    throw new Exception(mensaje);
                }

                return (unidades, mensaje, success);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las unidades.", ex);
            }
        }


        // Método para registrar una nueva unidad
        public async Task<(bool success, string mensaje, Unidad unidad)> RegistrarUnidadAsync(string unidadNombre)
        {
            try
            {
                var (success, unidadId, mensaje) = await _unidadDbContext.RegistrarUnidadAsync(unidadNombre);

                if (!success)
                {
                    return (false, mensaje, null);
                }

                var unidad = new Unidad
                {
                    UnidadId = unidadId,
                    UnidadNombre = unidadNombre,
                    UnidadFechaUltimaActualizacion = DateTime.Now
                };

                return (true, "Unidad registrada exitosamente.", unidad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la unidad.", ex);
            }
        }
    }
}