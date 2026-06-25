using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Controllers
{
    public class ClientePortalController : Controller
    {
        private readonly AppDbContext _context;

        public ClientePortalController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new ClienteDashboardViewModel
            {
                PlatillosDisponibles = await _context.Platillos.CountAsync(p => p.Disponibilidad),

                ReservasActivas = await _context.Reservas
                    .CountAsync(r => r.EstadoReserva == "Pendiente" || r.EstadoReserva == "Confirmada"),

                PedidosWebActivos = await _context.Pedidos
                    .CountAsync(p => p.TipoPedido == "Web - Recojo en local"
                                  || p.TipoPedido == "Web - Preorden"),

                MenuPlatillos = await _context.Platillos
                    .Include(p => p.CategoriaPlato)
                    .Where(p => p.Disponibilidad)
                    .OrderBy(p => p.IdCategoria)
                    .ToListAsync(),

                PedidosWeb = await _context.Pedidos
                    .Where(p => p.TipoPedido == "Web - Recojo en local"
                             || p.TipoPedido == "Web - Preorden")
                    .OrderByDescending(p => p.FechaPedido)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }
    }
}