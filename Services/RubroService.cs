using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystemBackend.Services
{
    public class RubroService
    {
        private readonly RubroDbContext _rubroDbContext;

        public RubroService(RubroDbContext rubroDbContext)
        {
            _rubroDbContext = rubroDbContext;
        }

        public async Task<List<Rubro>> ObtenerRubrosAsync()
        {
            try
            {
                return await _rubroDbContext.ObtenerRubrosAsync();
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
                return await _rubroDbContext.ObtenerRubroPorIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el rubro.", ex);
            }
        }

        public async Task<(bool success, string mensaje, Rubro rubro)> RegistrarRubroAsync(string rubroNombre)
        {
            try
            {
                var (success, rubroId, errorMessage) = await _rubroDbContext.RegistrarRubroAsync(rubroNombre);

                if (!success)
                {
                    return (false, errorMessage, null);
                }

                var rubro = new Rubro
                {
                    RubroId = rubroId,
                    RubroNombre = rubroNombre,
                    RubroFechaUltimaActualizacion = DateTime.Now
                };

                return (true, "Rubro registrado exitosamente.", rubro);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el rubro.", ex);
            }
        }
    }
}