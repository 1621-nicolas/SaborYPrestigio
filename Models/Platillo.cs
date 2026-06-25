using SaborYPrestigio.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Platillos")]
    public class Platillo
    {
        [Key]
        [Column("id_platillo")]
        public int IdPlatillo { get; set; }

        [Required]
        [Column("nombre_platillo")]
        public string NombrePlatillo { get; set; }

        [Column("descripcion_gourmet")]
        public string? DescripcionGourmet { get; set; }

        [Column("precio_venta")]
        public decimal PrecioVenta { get; set; }

        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Column("tiempo_estimado_min")]
        public int TiempoEstimadoMin { get; set; }

        [Column("disponibilidad")]
        public bool Disponibilidad { get; set; } = true;

        [ForeignKey("IdCategoria")]
        public CategoriaPlato? CategoriaPlato { get; set; }

        public ICollection<DetallePedido>? DetallePedidos { get; set; }
        public ICollection<RecetaPlatillo>? RecetasPlatillo { get; set; }
    }
}