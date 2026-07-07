using SaborYPrestigio.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Insumos")]
    public class Insumo
    {
        [Key]
        [Column("id_insumo")]
        public int IdInsumo { get; set; }

        [Required]
        [Column("nombre_insumo")]
        public string NombreInsumo { get; set; }

        [Required]
        [Column("unidad_medida")]
        public string UnidadMedida { get; set; }

        [Column("stock_actual")]
        public int StockActual { get; set; }

        [Column("stock_minimo")]
        public int StockMinimo { get; set; }

        [Column("precio_costo_promedio")]
        public decimal PrecioCostoPromedio { get; set; }

        public ICollection<RecetaPlatillo>? RecetasPlatillo { get; set; }
    }
}