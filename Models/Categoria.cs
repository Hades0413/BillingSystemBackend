using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models;

[Table("Categoria")]
public class Categoria
{
    [Key] [Column("categoria_id")] public int CategoriaId { get; set; }

    [Column("categoria_nombre")]
    [Required]
    [StringLength(255)]
    public string CategoriaNombre { get; set; }

    [Column("categoria_fecha_ultima_actualizacion")]
    public DateTime CategoriaFechaUltimaActualizacion { get; set; } = DateTime.Now;

    [Column("usuario_id")] public int UsuarioId { get; set; }
}