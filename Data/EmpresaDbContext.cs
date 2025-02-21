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
                return await Empresas.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las empresas.", ex);
            }
        }

        // Obtener empresa por ID
        public async Task<Empresa> ObtenerEmpresaPorIdAsync(int id)
        {
            try
            {
                return await Empresas.FirstOrDefaultAsync(e => e.EmpresaId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la empresa.", ex);
            }
        }

        public async Task<(bool success, int empresaId, string mensaje)> RegistrarEmpresaAsync(Empresa empresa)
        {
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

                await Database.ExecuteSqlRawAsync(
                    "EXEC dbo.InsertarEmpresa @usuario_id = {0}, @empresa_ruc = {1}, @empresa_razon_social = {2}, @empresa_nombre_comercial = {3}, @empresa_alias = {4}, @empresa_domicilio_fiscal = {5}, @empresa_logo = {6}, @rubro_id = {7}, @empresa_informacion_adicional = {8}, @empresaId = @empresaId OUTPUT, @mensaje = @mensaje OUTPUT",
                    empresa.UsuarioId, empresa.EmpresaRuc, empresa.EmpresaRazonSocial, empresa.EmpresaNombreComercial,
                    empresa.EmpresaAlias, empresa.EmpresaDomicilioFiscal, empresa.EmpresaLogo, empresa.RubroId,
                    empresa.EmpresaInformacionAdicional, empresaIdParam, mensajeParam);

                int empresaId = (int)empresaIdParam.Value;
                string mensaje = (string)mensajeParam.Value;

                return empresaId > 0
                    ? (true, empresaId, mensaje)
                    : (false, empresaId, mensaje);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar el procedimiento almacenado.", ex);
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
