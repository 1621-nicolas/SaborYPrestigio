using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Recetas_Platillo")]
    public class RecetaPlatillo
    {
        [Column("id_platillo")]
        public int IdPlatillo { get; set; }

        [Column("id_insumo")]
        public int IdInsumo { get; set; }

        [Column("cantidad_requerida")]
        public decimal CantidadRequerida { get; set; }

        [ForeignKey("IdPlatillo")]
        public Platillo? Platillo { get; set; }

        [ForeignKey("IdInsumo")]
        public Insumo? Insumo { get; set; }
    }
}