using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Detalle_Pedidos")]
    public class DetallePedido
    {
        [Column("id_pedido")]
        public long IdPedido { get; set; }

        [Column("id_platillo")]
        public int IdPlatillo { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }

        // AGREGA ESTA LÍNEA AQUÍ:
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
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