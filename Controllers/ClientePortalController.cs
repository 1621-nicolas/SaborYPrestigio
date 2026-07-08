using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;
using System.Security.Claims;

namespace SaborPrestigioMVC.Controllers
{
    // Solo clientes pueden hacer acciones aquí (excepto las que tengan AllowAnonymous)
    [Authorize(Roles = "Cliente")]
    public class ClientePortalController : Controller
    {
        private readonly AppDbContext _context;

        public ClientePortalController(AppDbContext context)
        {
            _context = context;
        }

        // 🔥 PERMITE ENTRAR SIN INICIAR SESIÓN (Para ver el menú)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var dashboard = new ClienteDashboardViewModel();

            // 1. INFO PÚBLICA: Cargamos el menú para todos (Invitados y Clientes)
            dashboard.PlatillosDisponibles = await _context.Platillos.CountAsync(p => p.Disponibilidad);
            dashboard.MenuPlatillos = await _context.Platillos
                .Include(p => p.CategoriaPlato)
                .Where(p => p.Disponibilidad)
                .OrderBy(p => p.IdCategoria)
                .ToListAsync();

            // 2. INFO PRIVADA: Solo cargamos reservas y pedidos si el usuario SÍ inició sesión
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Extraemos el ID del cliente logueado (que guardamos en la cookie del Login)
                var claimId = User.FindFirst("UsuarioId")?.Value;
                long idCliente = string.IsNullOrEmpty(claimId) ? 0 : long.Parse(claimId);

                // 🔥 Modificado para que solo cuente los pedidos/reservas de ESTE cliente en específico
                dashboard.ReservasActivas = await _context.Reservas
                    .CountAsync(r => r.IdCliente == idCliente && (r.EstadoReserva == "Pendiente" || r.EstadoReserva == "Confirmada"));

                dashboard.PedidosWebActivos = await _context.Pedidos
                    .CountAsync(p => p.IdCliente == idCliente && (p.TipoPedido == "Web - Recojo en local" || p.TipoPedido == "Web - Preorden"));

                dashboard.PedidosWeb = await _context.Pedidos
                    .Where(p => p.IdCliente == idCliente && (p.TipoPedido == "Web - Recojo en local" || p.TipoPedido == "Web - Preorden"))
                    .OrderByDescending(p => p.FechaPedido)
                    .Take(5)
                    .ToListAsync();
            }
            else
            {
                // Si es invitado, inicializamos en 0 para que la vista no se caiga
                dashboard.ReservasActivas = 0;
                dashboard.PedidosWebActivos = 0;
                dashboard.PedidosWeb = new List<Pedido>();
            }

            return View(dashboard);
        }
    }
}