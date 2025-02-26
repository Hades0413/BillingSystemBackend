using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystemBackend.Services;

public class UsuarioService
{
    private readonly UsuarioDbContext _context;

    public UsuarioService(UsuarioDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegistrarUsuarioAsync(Usuario usuario)
    {
        if (await _context.Usuarios.AnyAsync(u => u.UsuarioCorreo == usuario.UsuarioCorreo)) return false;

        if (usuario.UsuarioContrasena.Length < 6)
            throw new InvalidOperationException("La contraseña debe tener al menos 6 caracteres.");

        usuario.UsuarioContrasena = BCrypt.Net.BCrypt.HashPassword(usuario.UsuarioContrasena);

        usuario.UsuarioFechaUltimaActualizacion = DateTime.Now;

        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Usuario> ObtenerUsuarioPorIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario> ObtenerUsuarioConCorreoAsync(string correo)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioCorreo == correo);
    }
    
    public async Task<List<Usuario>> ObtenerTodosUsuariosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }
    
    public async Task<bool> ActualizarUsuarioAsync(Usuario usuario)
    {
        var usuarioExistente = await _context.Usuarios.FindAsync(usuario.UsuarioId);
        if (usuarioExistente == null)
            return false; 
        
        usuarioExistente.UsuarioCorreo = usuario.UsuarioCorreo;
        usuarioExistente.UsuarioContrasena = usuario.UsuarioContrasena;
        usuarioExistente.UsuarioTelefono = usuario.UsuarioTelefono;
        usuarioExistente.UsuarioNombres = usuario.UsuarioNombres;
        usuarioExistente.UsuarioApellidos = usuario.UsuarioApellidos;
        usuarioExistente.UsuarioFechaUltimaActualizacion = usuario.UsuarioFechaUltimaActualizacion;

        _context.Usuarios.Update(usuarioExistente);
        await _context.SaveChangesAsync();
        return true;
    }

    
}