using SaborYPrestigio.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaborPrestigioMVC.Models
{
    [Table("Categorias_Platos")]
    public class CategoriaPlato
    {
        [Key]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [Column("nombre_categoria")]
        public string NombreCategoria { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        public ICollection<Platillo>? Platillos { get; set; }
    }
}

    
