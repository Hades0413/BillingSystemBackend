using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using BillingSystemBackend.Models;

namespace BillingSystemBackend.Data
{
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

        public async Task<(bool success, int empresaId, string mensaje)> RegistrarEmpresaAsync(Empresa empresa)
{
    if (empresa == null)
        throw new ArgumentNullException(nameof(empresa), "La empresa no puede ser nula.");

    if (empresa.UsuarioId <= 0)
        throw new ArgumentException("El ID del usuario debe ser mayor a cero.", nameof(empresa.UsuarioId));

    if (string.IsNullOrEmpty(empresa.EmpresaRazonSocial))
        throw new ArgumentException("La razón social de la empresa no puede estar vacía.", nameof(empresa.EmpresaRazonSocial));

    try
    {
        var empresaIdParam = new SqlParameter("@empresaId", SqlDbType.Int)
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
                if (!string.IsNullOrEmpty(empresa.EmpresaRuc))
                {
                    var existeRuc = await Empresas
                        .AnyAsync(e => e.EmpresaRuc == empresa.EmpresaRuc);

                    if (existeRuc)
                    {
                        mensajeParam.Value = "Ya existe una empresa con este RUC.";
                        empresaIdParam.Value = 0;
                        return (false, 0, "Ya existe una empresa con este RUC.");
                    }
                }

                await Database.ExecuteSqlRawAsync(
                    "EXEC dbo.InsertarEmpresa @usuario_id = {0}, @empresa_ruc = {1}, @empresa_razon_social = {2}, " +
                    "@empresa_nombre_comercial = {3}, @empresa_alias = {4}, @empresa_domicilio_fiscal = {5}, " +
                    "@empresa_logo = {6}, @rubro_id = {7}, @empresa_informacion_adicional = {8}, " +
                    "@empresaId = @empresaId OUTPUT, @mensaje = @mensaje OUTPUT",
                    empresa.UsuarioId, empresa.EmpresaRuc, empresa.EmpresaRazonSocial, empresa.EmpresaNombreComercial,
                    empresa.EmpresaAlias, empresa.EmpresaDomicilioFiscal, empresa.EmpresaLogo, empresa.RubroId,
                    empresa.EmpresaInformacionAdicional, empresaIdParam, mensajeParam);

                int empresaId = (int)empresaIdParam.Value;
                string mensaje = (string)mensajeParam.Value;

                await transaction.CommitAsync();

                return empresaId > 0
                    ? (true, empresaId, mensaje)
                    : (false, empresaId, mensaje);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Error al registrar la empresa, la transacción fue revertida.", ex);
            }
        }
    }
    catch (Exception ex)
    {
        throw new Exception("Error al ejecutar el procedimiento almacenado para registrar la empresa.", ex);
    }
}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Empresa>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Empresa>()
                .HasOne<Rubro>()
                .WithMany()
                .HasForeignKey(e => e.RubroId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
