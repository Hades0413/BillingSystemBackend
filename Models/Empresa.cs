using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models
{
    [Table("Empresa")]
    public class Empresa
    {
        [Key]
        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("empresa_ruc")]
        [StringLength(20)]
        public string EmpresaRuc { get; set; }

        [Column("empresa_razon_social")]
        [Required]
        [StringLength(255)]
        public string EmpresaRazonSocial { get; set; }

        [Column("empresa_nombre_comercial")]
        [StringLength(255)]
        public string EmpresaNombreComercial { get; set; }

        [Column("empresa_alias")]
        [StringLength(100)]
        public string EmpresaAlias { get; set; }

        [Column("empresa_domicilio_fiscal")]
        [Required]
        [StringLength(500)]
        public string EmpresaDomicilioFiscal { get; set; }

        [Column("empresa_logo")]
        [StringLength(500)]
        public string EmpresaLogo { get; set; }

        [Column("rubro_id")]
        public int RubroId { get; set; }

        [Column("empresa_informacion_adicional")]
        public string EmpresaInformacionAdicional { get; set; }

        [Column("empresa_fecha_ultima_actualizacion")]
        public DateTime EmpresaFechaUltimaActualizacion { get; set; } = DateTime.Now;

    }
}