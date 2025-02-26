using BillingSystemBackend.Data;
using BillingSystemBackend.Models;

namespace BillingSystemBackend.Services;

public class CategoriaService
{
    private readonly CategoriaDbContext _categoriaDbContext;

    public CategoriaService(CategoriaDbContext categoriaDbContext)
    {
        _categoriaDbContext = categoriaDbContext;
    }

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

    public async Task<(bool success, string mensaje, Categoria categoria)> RegistrarCategoriaAsync(int usuarioId,
        string categoriaNombre)
    {
        try
        {
            var (success, categoriaId, errorMessage) =
                await _categoriaDbContext.RegistrarCategoriaAsync(usuarioId, categoriaNombre);

            if (!success) return (false, errorMessage, null);

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

    public async Task<(bool success, string message, Categoria categoria)> EditarCategoriaAsync(int categoriaId,
        Categoria categoria)
    {
        try
        {
            var categoriaExistente = await _categoriaDbContext.ObtenerCategoriaPorIdAsync(categoriaId);

            if (categoriaExistente == null) return (false, "La categoría no existe.", null);

            categoriaExistente.CategoriaNombre = categoria.CategoriaNombre;
            categoriaExistente.CategoriaFechaUltimaActualizacion = DateTime.Now;

            _categoriaDbContext.Update(categoriaExistente);
            await _categoriaDbContext.SaveChangesAsync();

            return (true, "Categoría actualizada exitosamente.", categoriaExistente);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al actualizar la categoría.", ex);
        }
    }

    public async Task<(bool success, string message)> EliminarCategoriaAsync(int categoriaId)
    {
        try
        {
            var filasAfectadas = await _categoriaDbContext.EliminarCategoriaAsync(categoriaId);

            if (filasAfectadas == 0) return (false, "La categoría tiene productos asociados y no se puede eliminar.");

            return (true, "Categoría eliminada exitosamente.");
        }
        catch (Exception ex)
        {
            return (false, $"Error al eliminar la categoría: {ex.Message}");
        }
    }
}