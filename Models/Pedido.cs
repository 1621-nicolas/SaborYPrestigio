using SaborYPrestigio.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        [Column("id_pedido")]
        public long IdPedido { get; set; }

        [Column("id_cliente")]
        public long? IdCliente { get; set; }

        [Column("id_empleado")]
        public int? IdEmpleado { get; set; }

        [Column("id_mesa")]
        public int? IdMesa { get; set; }

        [Required]
        [Column("tipo_pedido")]
        public string TipoPedido { get; set; }

        [Column("fecha_pedido")]
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        [Column("estado_pedido")]
        public string EstadoPedido { get; set; } = "En Espera";

        [Column("total")]
        public decimal Total { get; set; } = 0;

        [ForeignKey("IdCliente")]
        public Cliente? Cliente { get; set; }

        [ForeignKey("IdEmpleado")]
        public Empleado? Empleado { get; set; }

        [ForeignKey("IdMesa")]
        public Mesa? Mesa { get; set; }

        public ICollection<DetallePedido>? DetallePedidos { get; set; }
        public ComprobantePago? ComprobantePago { get; set; }
    }
}