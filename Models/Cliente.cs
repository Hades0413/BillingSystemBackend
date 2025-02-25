using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        [Column("cliente_id")]
        public int ClienteId { get; set; }
        [Column("tipo_cliente_id")]
        public int TipoClienteId { get; set; }
        [Column("cliente_ruc")]
        public string ClienteRuc { get; set; }
        [Column("cliente_dni")]
        public string ClienteDni { get; set; }
        [Column("cliente_nombrelegal")]
        public string ClienteNombreLegal { get; set; }
        [Column("cliente_direccion")]
        public string ClienteDireccion { get; set; }
        [Column("cliente_fecha_creacion")]
        public DateTime ClienteFechaCreacion { get; set; } = DateTime.Now;
    }
}
