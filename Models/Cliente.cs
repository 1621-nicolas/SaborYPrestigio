using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public long IdCliente { get; set; }

        [Required(ErrorMessage = "Ingrese DNI/RUC")]
        [RegularExpression(@"^\d{8}$",
            ErrorMessage = "El DNI debe tener exactamente 8 dígitos")]
        [Column("dni_ruc")]
        public string DniRuc { get; set; }

        [Required(ErrorMessage = "Ingrese nombre")]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Ingrese apellido")]
        [Column("apellido")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "Ingrese teléfono")]
        [RegularExpression(@"^\d{9}$",
            ErrorMessage = "El teléfono debe tener exactamente 9 dígitos")]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "Ingrese correo")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [Column("email")]
        public string Email { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Reserva>? Reservas { get; set; }
        public ICollection<Pedido>? Pedidos { get; set; }
    }
}