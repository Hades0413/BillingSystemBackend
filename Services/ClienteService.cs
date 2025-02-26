using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Services;

public class ClienteService
{
    private readonly ClienteDbContext _context;

    public ClienteService(ClienteDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente> RegistrarClienteAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task<List<Cliente>> ListarClientesAsync()
    {
        return await _context.Clientes.ToListAsync();
    }

    public async Task<Cliente> ListarClientePorIdAsync(int clienteId)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
    }

    public async Task<List<Cliente>> ListarClientesPorNombreLegalAsync(string nombreLegal)
    {
        return await _context.Clientes
            .Where(c => c.ClienteNombreLegal.Contains(nombreLegal))
            .ToListAsync();
    }
}