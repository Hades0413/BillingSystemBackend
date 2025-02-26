using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models;

public class Unidad
{
    [Key] [Column("unidad_id")] public int UnidadId { get; set; }

    [Column("unidad_nombre")]
    [Required]
    [StringLength(255)]
    public string UnidadNombre { get; set; }

    [Column("unidad_fecha_ultima_actualizacion")]
    public DateTime UnidadFechaUltimaActualizacion { get; set; } = DateTime.Now;
}