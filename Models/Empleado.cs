using SaborYPrestigio.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Empleados")]
    public class Empleado
    {
        [Key]
        [Column("id_empleado")]
        public int IdEmpleado { get; set; }

        [Required]
        [Column("dni")]
        public string Dni { get; set; } = string.Empty;

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("correo")]
        public string? Correo { get; set; }

        [Required]
        [Column("usuario")]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        [Column("contrasenia_hash")]
        public string ContraseniaHash { get; set; } = string.Empty;

        [Column("id_rol")]
        public int IdRol { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = "Activo";

        [ForeignKey("IdRol")]
        public Rol? Rol { get; set; }

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}