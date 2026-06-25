using SaborYPrestigio.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Mesas")]
    public class Mesa
    {
        [Key]
        [Column("id_mesa")]
        public int IdMesa { get; set; }

        [Required]
        [Range(1, 999)]
        [Column("numero_mesa")]
        public int NumeroMesa { get; set; }

        [Required]
        [Range(1, 20,
         ErrorMessage = "La capacidad debe estar entre 1 y 20 personas")]
        [Column("capacidad")]
        public int Capacidad { get; set; }

        [Required(ErrorMessage = "La zona es obligatoria")]
        [StringLength(50)]
        [Column("zona")]
        public string Zona { get; set; }

        [Required]
        [Column("estado")]
        public string Estado { get; set; } = "Disponible";

        public ICollection<Reserva>? Reservas { get; set; }
        public ICollection<Pedido>? Pedidos { get; set; }
    }
}