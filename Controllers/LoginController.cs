using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;
using SaborYPrestigio.Models;
using System.Linq;
using System.Threading.Tasks;
using static SaborYPrestigio.Models.LoginViewModel;

namespace SaborYPrestigio.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Autenticar([FromBody] LoginViewModel model)
        {
            var response = new LoginResponse { Exito = false };

            if (model.TipoAcceso == "EMPLEADO")
            {
                var empleado = await _context.Empleados 
                    .Include(e => e.Rol)
                    .FirstOrDefaultAsync(e => e.Usuario == model.Usuario && e.ContraseniaHash == model.Password && e.Estado == "Activo");

                if (empleado != null)
                {
                    response.Exito = true;
                    response.NombreCompleto = $"{empleado.Nombre} {empleado.Apellido}";
                    response.Rol = empleado.Rol.NombreRol; 
                    response.UrlRedireccion = "/Home/Index"; 
                }
                else
                {
                    response.Mensaje = "Credenciales incorrectas o usuario inactivo.";
                }
            }
            else if (model.TipoAcceso == "CLIENTE")
            {
                
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Email == model.Usuario && c.ContraseniaHash == model.Password);

                if (cliente != null)
                {
                    response.Exito = true;
                    response.NombreCompleto = $"{cliente.Nombre} {cliente.Apellido}";
                    response.Rol = "Cliente";
                    response.UrlRedireccion = "/ClientePortal/Index";
                }
                else
                {
                    response.Mensaje = "Correo o contraseña no válidos.";
                }
            }
            return Json(response);
        }

        // 1. Método para cargar la interfaz HTML de Registro
        [HttpGet]
        public IActionResult RegistrarCliente()
        {
            return View();
        }

        // 2. Método API para guardar el cliente en SQL Server
        [HttpPost]
        public async Task<JsonResult> Registrar([FromBody] RegistroClienteViewModel model)
        {
            var response = new LoginResponse { Exito = false };

            try
            {
                // Validación 1: Verificar si el correo o DNI ya existen
                bool existe = await _context.Clientes
                    .AnyAsync(c => c.Email == model.Email || c.DniRuc == model.DniRuc);

                if (existe)
                {
                    response.Mensaje = "El correo o DNI/RUC ya se encuentra registrado.";
                    return Json(response);
                }

                // Creación de la entidad según su modelo Cliente.cs
                var nuevoCliente = new Cliente
                {
                    DniRuc = model.DniRuc,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Telefono = model.Telefono,
                    Email = model.Email,
                    ContraseniaHash = model.Password, // En un sistema real de producción, esto iría encriptado
                    FechaRegistro = DateTime.Now
                };

                _context.Clientes.Add(nuevoCliente);
                await _context.SaveChangesAsync();

                response.Exito = true;
                response.Mensaje = "Cuenta creada exitosamente. Ahora puede iniciar sesión.";
                response.UrlRedireccion = "/Login/Index";
            }
            catch (Exception ex)
            {
                response.Mensaje = "Error en el servidor al registrar: " + ex.Message;
            }

            return Json(response);
        }
    }
}
