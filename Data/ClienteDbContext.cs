using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Data;

public class ClienteDbContext : DbContext
{
    public ClienteDbContext(DbContextOptions<ClienteDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<TipoCliente> TipoClientes { get; set; }
}