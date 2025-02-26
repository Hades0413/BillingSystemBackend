using System.Data;
using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Services;

public class EmpresaService
{
    private readonly EmpresaDbContext _context;

    public EmpresaService(EmpresaDbContext context)
    {
        _context = context;
    }

    public async Task<Empresa> ObtenerEmpresaPorIdAsync(int id)
    {
        return await _context.Empresas
            .FirstOrDefaultAsync(e => e.EmpresaId == id);
    }

    public async Task<(int empresaId, string mensaje)> RegistrarEmpresaAsync(Empresa empresa)
    {
        if (!string.IsNullOrEmpty(empresa.EmpresaRuc))
            if (await _context.Empresas.AnyAsync(e => e.EmpresaRuc == empresa.EmpresaRuc))
                return (0, "Ya existe una empresa con este RUC.");

        if (empresa.UsuarioId == 0 || empresa.RubroId == 0) return (0, "UsuarioId y RubroId son obligatorios.");

        var empresaIdParam = new SqlParameter("@empresaId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var mensajeParam = new SqlParameter("@mensaje", SqlDbType.NVarChar, 255)
            { Direction = ParameterDirection.Output };

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.InsertarEmpresa @usuario_id = {0}, @empresa_ruc = {1}, @empresa_razon_social = {2}, @empresa_nombre_comercial = {3}, @empresa_alias = {4}, @empresa_domicilio_fiscal = {5}, @empresa_logo = {6}, @rubro_id = {7}, @empresa_informacion_adicional = {8}, @empresaId = @empresaId OUTPUT, @mensaje = @mensaje OUTPUT",
            empresa.UsuarioId, empresa.EmpresaRuc, empresa.EmpresaRazonSocial, empresa.EmpresaNombreComercial,
            empresa.EmpresaAlias, empresa.EmpresaDomicilioFiscal, empresa.EmpresaLogo, empresa.RubroId,
            empresa.EmpresaInformacionAdicional, empresaIdParam, mensajeParam);

        var empresaId = (int)empresaIdParam.Value;
        var mensaje = (string)mensajeParam.Value;

        return (empresaId, mensaje);
    }

    public async Task<List<Empresa>> ListarEmpresasPorUsuarioIdAsync(int usuarioId)
    {
        return await _context.ListarEmpresasPorUsuarioIdAsync(usuarioId);
    }

    public async Task<List<Empresa>> ListarEmpresasPorRucAsync(string ruc)
    {
        if (string.IsNullOrWhiteSpace(ruc)) throw new ArgumentException("El RUC no puede estar vacío.", nameof(ruc));

        return await _context.ListarEmpresasPorRucAsync(ruc);
    }
    
    public async Task<List<Empresa>> ObtenerEmpresasAsync()
    {
        try
        {
            return await _context.Empresas.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las empresas.", ex);
        }
    }

}