using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystemBackend.Models;

[Table("Usuario")]
public class Usuario
{
    [Key] [Column("usuario_id")] public int UsuarioId { get; set; }

    [Column("usuario_correo")]
    [EmailAddress]
    [Required]
    public string UsuarioCorreo { get; set; }

    [Column("usuario_contrasena")]
    [Required]
    [MinLength(6)]
    public string UsuarioContrasena { get; set; }

    [Column("usuario_telefono")] public string UsuarioTelefono { get; set; }

    [Column("usuario_nombres")] public string UsuarioNombres { get; set; }

    [Column("usuario_apellidos")] public string UsuarioApellidos { get; set; }

    [Column("usuario_fecha_ultima_actualizacion")]
    public DateTime UsuarioFechaUltimaActualizacion { get; set; } = DateTime.Now;
}