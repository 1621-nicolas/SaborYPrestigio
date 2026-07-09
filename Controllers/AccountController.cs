using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;
using SaborYPrestigio.Models;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // 1. Obtener el rol de la cookie
            var rol = User.FindFirstValue(ClaimTypes.Role);

            // 2. Verificar si el rol tiene acceso a lo que le corresponde
            return RedirigirSegunRol(rol);
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // 1. Buscar en Empleados usando los campos exactos de tu modelo
        var empleado = await _context.Empleados
            .Include(e => e.Rol) // Traemos la tabla relacionada "Rol" para saber si es Admin
            .FirstOrDefaultAsync(e =>
                (e.Usuario == model.Usuario || e.Correo == model.Usuario || e.Dni == model.Usuario)
                && e.ContraseniaHash == model.Clave); // Compara con tu propiedad ContraseniaHash

        if (empleado != null)
        {
            // Asignar rol. *NOTA: Cambia "Nombre" por la propiedad exacta que tenga tu clase Rol (ej. NombreRol, Descripcion, etc.)
            bool esAdmin = empleado.Rol != null && empleado.Rol.NombreRol.Equals("Administrador", StringComparison.OrdinalIgnoreCase);

            string rol = esAdmin ? "Administrador" : "Trabajador";

            await IniciarSesionCookie(empleado.Nombre, empleado.Correo ?? empleado.Usuario, rol, empleado.IdEmpleado.ToString());
            return RedirigirSegunRol(rol);
        }

        // 2. Si no es empleado, buscar en Clientes (requiere el campo Clave que agregamos en el paso 1)
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email == model.Usuario && c.Clave == model.Clave);

        if (cliente != null)
        {
            await IniciarSesionCookie(cliente.Nombre, cliente.Email, "Cliente", cliente.IdCliente.ToString());
            return RedirigirSegunRol("Cliente");
        }

        ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
        return View(model);
    }

    private async Task IniciarSesionCookie(string nombre, string email, string rol, string id)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nombre),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, rol),
            new Claim("UsuarioId", id)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
    }

    private IActionResult RedirigirSegunRol(string? rol)
    {
        return rol switch
    {
        "Administrador" => RedirectToAction("Index", "Home"),
        "Mozo"          => RedirectToAction("Index", "Pedidos"),
        "Cajero"        => RedirectToAction("Index", "ComprobantedePago"),
        "Chef"          => RedirectToAction("Index", "Pedidos"), // O a donde prefieras
        "Cliente"       => RedirectToAction("Index", "ClientePortal"),
        _               => RedirectToAction("Login")
    };
    }

    // GET: /Account/Register
    [HttpGet]
    [AllowAnonymous] // Cualquiera puede ver la pantalla de registro
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register([Bind("DniRuc,Nombre,Apellido,Telefono,Email,Clave")] Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            // Verificamos que el correo no exista ya
            var existe = await _context.Clientes.AnyAsync(c => c.Email == cliente.Email);
            if (existe)
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado.");
                return View(cliente);
            }

            cliente.FechaRegistro = DateTime.Now;
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            // Si se registra con éxito, lo mandamos al Login para que entre
            return RedirectToAction(nameof(Login));
        }
        return View(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}