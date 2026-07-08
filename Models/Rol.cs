using SaborYPrestigio.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models;
    [Table("Roles")]
    public class Rol
    {
        [Key]
        [Column("id_rol")]
        public int IdRol { get; set; }

        [Required]
        [Column("nombre_rol")]
        public string NombreRol { get; set; } = null!;

    [Column("descripcion")]
        public string? Descripcion { get; set; }

        public ICollection<Empleado>? Empleados { get; set; }
    }
    