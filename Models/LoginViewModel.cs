using System.ComponentModel.DataAnnotations;

namespace SaborYPrestigio.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo o usuario es obligatorio")]
        public string Usuario { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = null!;
    }
}
