using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models;

[Table("Rubro")]
public class Rubro
{
    [Key] [Column("rubro_id")] public int RubroId { get; set; }

    [Column("rubro_nombre")]
    [Required]
    [StringLength(50)]
    public string RubroNombre { get; set; }

    [Column("rubro_fecha_ultima_actualizacion")]
    public DateTime RubroFechaUltimaActualizacion { get; set; } = DateTime.Now;
}