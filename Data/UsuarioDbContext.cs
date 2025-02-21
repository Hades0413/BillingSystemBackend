using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Models
{
    public class UsuarioDbContext : DbContext
    {
        public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.UsuarioCorreo)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.UsuarioContrasena)
                .HasMaxLength(255)
                .IsRequired();
        }
    }
}