using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Services
{
    public class TipoClienteService
    {
        private readonly ClienteDbContext _context;

        public TipoClienteService(ClienteDbContext context)
        {
            _context = context;
        }

        public async Task<TipoCliente> RegistrarTipoClienteAsync(TipoCliente tipoCliente)
        {
            _context.TipoClientes.Add(tipoCliente);
            await _context.SaveChangesAsync();
            return tipoCliente;
        }

        public async Task<List<TipoCliente>> ListarTipoClientesAsync()
        {
            return await _context.TipoClientes.ToListAsync();
        }

        public async Task<TipoCliente> ListarTipoClientePorIdAsync(int tipoClienteId)
        {
            return await _context.TipoClientes
                .FirstOrDefaultAsync(t => t.TipoClienteId == tipoClienteId);
        }

        public async Task<List<TipoCliente>> ListarTipoClientePorNombreAsync(string nombre)
        {
            return await _context.TipoClientes
                .Where(t => t.TipoClienteNombre.Contains(nombre))
                .ToListAsync();
        }
    }
}