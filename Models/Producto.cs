using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models;

[Table("Producto")]
public class Producto
{
    [Key] [Column("producto_id")] public int ProductoId { get; set; }

    [Column("producto_codigo")]
    [Required]
    [StringLength(50)]
    public string ProductoCodigo { get; set; }

    [Column("producto_nombre")]
    [Required]
    [StringLength(255)]
    public string ProductoNombre { get; set; }

    [Column("producto_stock")] public int ProductoStock { get; set; }

    [Column("producto_precio_venta")] public decimal ProductoPrecioVenta { get; set; }

    [Column("producto_impuesto_igv")] public decimal? ProductoImpuestoIgv { get; set; }

    [Column("unidad_id")] public int UnidadId { get; set; }

    [Column("categoria_id")] public int CategoriaId { get; set; }

    [Column("usuario_id")] public int UsuarioId { get; set; }

    [Column("producto_imagen")] public string ProductoImagen { get; set; }

    [Column("producto_fecha_ultima_actualizacion")]
    public DateTime ProductoFechaUltimaActualizacion { get; set; } = DateTime.Now;
}