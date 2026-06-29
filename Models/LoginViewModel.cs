namespace SaborYPrestigio.Models
{
    public class LoginViewModel
    {
        public string TipoAcceso { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }

        public class LoginResponse
        {
            public bool Exito { get; set; }
            public string Mensaje { get; set; }
            public string Rol { get; set; }
            public string NombreCompleto { get; set; }
            public string UrlRedireccion { get; set; }
        }
        public class RegistroClienteViewModel
        {
            public string DniRuc { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Telefono { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
