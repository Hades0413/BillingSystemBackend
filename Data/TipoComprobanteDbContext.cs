using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Data
{
    public class TipoComprobanteDbContext : DbContext
    {
        public TipoComprobanteDbContext(DbContextOptions<TipoComprobanteDbContext> options) : base(options)
        {
        }

        public DbSet<TipoComprobante> TipoComprobantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TipoComprobante>()
                .HasKey(t => t.TipoComprobanteId);
        }
    }
}