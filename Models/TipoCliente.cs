using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models
{
    [Table("TipoCliente")]
    public class TipoCliente
    {
        [Key]
        [Column("tipo_cliente_id")]
        public int TipoClienteId { get; set; }
        
        [Column("tipo_cliente_nombre")]
        public string TipoClienteNombre { get; set; }
    }
}