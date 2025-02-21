using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingSystemBackend.Services
{
    public class CategoriaService
    {
        private readonly CategoriaDbContext _categoriaDbContext;

        public CategoriaService(CategoriaDbContext categoriaDbContext)
        {
            _categoriaDbContext = categoriaDbContext;
        }

        // Método para listar las categorías de un usuario
        public async Task<List<Categoria>> ListarCategoriasConUsuarioIdAsync(int usuarioId)
        {
            try
            {
                return await _categoriaDbContext.ListarCategoriasConUsuarioIdAsync(usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las categorías.", ex);
            }
        }

        // Obtener una categoría por su ID
        public async Task<Categoria> ObtenerCategoriaPorIdAsync(int id)
        {
            try
            {
                return await _categoriaDbContext.ObtenerCategoriaPorIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la categoría.", ex);
            }
        }

        // Método para registrar una nueva categoría
        public async Task<(bool success, string mensaje, Categoria categoria)> RegistrarCategoriaAsync(int usuarioId, string categoriaNombre)
        {
            try
            {
                var (success, categoriaId, errorMessage) = await _categoriaDbContext.RegistrarCategoriaAsync(usuarioId, categoriaNombre);

                if (!success)
                {
                    return (false, errorMessage, null);
                }

                var categoria = new Categoria
                {
                    CategoriaId = categoriaId,
                    CategoriaNombre = categoriaNombre,
                    UsuarioId = usuarioId,
                    CategoriaFechaUltimaActualizacion = DateTime.Now
                };

                return (true, "Categoría registrada exitosamente.", categoria);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la categoría.", ex);
            }
        }
    }
}
