using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Services;

public class TipoComprobanteService
{
    private readonly TipoComprobanteDbContext _dbContext;

    public TipoComprobanteService(TipoComprobanteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(bool Success, List<TipoComprobante> Data, string Message)> ObtenerTipoComprobantesAsync()
    {
        try
        {
            var tipoComprobantes = await _dbContext.TipoComprobantes.ToListAsync();

            if (tipoComprobantes == null || tipoComprobantes.Count == 0)
                return (false, null, "No se encontraron tipos de comprobante.");

            return (true, tipoComprobantes, "Tipos de comprobante obtenidos con éxito.");
        }
        catch (Exception ex)
        {
            return (false, null, $"Ocurrió un error inesperado: {ex.Message}");
        }
    }
}