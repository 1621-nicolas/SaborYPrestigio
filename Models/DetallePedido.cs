using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Detalle_Pedidos")]
    public class DetallePedido
    {
        [Key]
        [Column("id_detalle")]
        public long IdDetalle { get; set; }

        [Column("id_pedido")]
        public long IdPedido { get; set; }

        [Column("id_platillo")]
        public int IdPlatillo { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; private set; }

        [Column("notas_chef")]
        public string? NotasChef { get; set; }

        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }

        [ForeignKey("IdPlatillo")]
        public Platillo? Platillo { get; set; }
    }
}