using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Comprobantes_Pago")]
    public class ComprobantePago
    {
        [Key]
        [Column("id_comprobante")]
        public long IdComprobante { get; set; }

        [Column("id_pedido")]
        public long IdPedido { get; set; }

        [Required]
        [Column("tipo_comprobante")]
        public string TipoComprobante { get; set; }

        [Required]
        [Column("serie")]
        public string Serie { get; set; }

        [Column("correlativo")]
        public int Correlativo { get; set; }

        [Column("fecha_emision")]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [Required]
        [Column("metodo_pago")]
        public string MetodoPago { get; set; }

        [Column("monto_subtotal")]
        public decimal MontoSubtotal { get; set; }

        [Column("monto_igv")]
        public decimal MontoIgv { get; set; }

        [Column("monto_total")]
        public decimal MontoTotal { get; set; }

        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }
        [Column("cliente_documento")]
        public string ClienteDocumento { get; set; }

        [Column("cliente_nombre_o_razon_social")]
        public string ClienteNombreORazonSocial { get; set; }
    }
}