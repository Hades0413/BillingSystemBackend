using BCrypt.Net;
using BillingSystemBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystemBackend.Services
{
    public class UsuarioService
    {
        private readonly AuthDbContext _context;

        public UsuarioService(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegistrarUsuarioAsync(Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.UsuarioCorreo == usuario.UsuarioCorreo))
            {
                return false;
            }

            if (usuario.UsuarioContrasena.Length < 6)
            {
                throw new InvalidOperationException("La contraseña debe tener al menos 6 caracteres.");
            }

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
    }
}