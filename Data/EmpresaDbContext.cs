using System.Data;
using BillingSystemBackend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Data;

public class EmpresaDbContext : DbContext
{
    public EmpresaDbContext(DbContextOptions<EmpresaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Empresa> Empresas { get; set; }

    public async Task<List<Empresa>> ObtenerEmpresasAsync()
    {
        try
        {
            return await Empresas.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las empresas.", ex);
        }
    }

    public async Task<Empresa> ObtenerEmpresaPorIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El ID de la empresa debe ser mayor a cero.", nameof(id));

        try
        {
            return await Empresas.AsNoTracking().FirstOrDefaultAsync(e => e.EmpresaId == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener la empresa con ID {id}.", ex);
        }
    }

    public async Task<List<Empresa>> ListarEmpresasPorUsuarioIdAsync(int usuarioId)
    {
        try
        {
            var sqlParams = new SqlParameter("@usuario_id", SqlDbType.Int) { Value = usuarioId };

            return await Empresas.FromSqlRaw("EXEC dbo.ListarEmpresasConUsuarioId @usuario_id", sqlParams)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al ejecutar el procedimiento almacenado para listar las empresas.", ex);
        }
    }

    public async Task<List<Empresa>> ListarEmpresasPorRucAsync(string ruc)
    {
        try
        {
            var sqlParams = new SqlParameter("@empresa_ruc", SqlDbType.NVarChar, 20) { Value = ruc };
            return await Empresas.FromSqlRaw("EXEC dbo.ListarEmpresasPorRUC @empresa_ruc", sqlParams).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al ejecutar el procedimiento almacenado para listar las empresas por RUC.", ex);
        }
    }
}