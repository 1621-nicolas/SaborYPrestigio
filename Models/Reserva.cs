using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        [Column("id_reserva")]
        public long IdReserva { get; set; }

        [Column("id_cliente")]
        public long IdCliente { get; set; }

        [Column("id_mesa")]
        public int? IdMesa { get; set; }

        [Column("fecha_reserva")]
        public DateTime FechaReserva { get; set; }

        [Column("hora_reserva")]
        public TimeSpan HoraReserva { get; set; }

        [Column("cantidad_personas")]
        [Required(ErrorMessage = "Ingrese la cantidad de personas")]
        [Range(1, 20, ErrorMessage = "La cantidad debe ser entre 1 y 20")]
   
        public int CantidadPersonas { get; set; }
        [Required]
        [Column("origen")]
        public string Origen { get; set; }

        [Column("estado_reserva")]
        public string EstadoReserva { get; set; } = "Pendiente";

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [ForeignKey("IdCliente")]
        public Cliente? Cliente { get; set; }

        [ForeignKey("IdMesa")]
        public Mesa? Mesa { get; set; }
    }
}