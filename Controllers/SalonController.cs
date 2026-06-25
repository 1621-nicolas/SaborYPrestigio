using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Controllers
{
    public class SalonController : Controller
    {
        private readonly AppDbContext _context;

        public SalonController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;

            var dashboard = new SalonDashboardViewModel
            {
                MesasDisponibles = await _context.Mesas.CountAsync(m => m.Estado == "Disponible"),
                MesasOcupadas = await _context.Mesas.CountAsync(m => m.Estado == "Ocupada"),
                ReservasDelDia = await _context.Reservas.CountAsync(r => r.FechaReserva.Date == hoy),
                PedidosActivos = await _context.Pedidos.CountAsync(p => p.EstadoPedido != "Entregado"),

                Mesas = await _context.Mesas.ToListAsync(),

                ReservasHoy = await _context.Reservas
                    .Include(r => r.Cliente)
                    .Include(r => r.Mesa)
                    .Where(r => r.FechaReserva.Date == hoy)
                    .ToListAsync(),

                PedidosActivosLista = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Mesa)
                    .Where(p => p.EstadoPedido != "Entregado")
                    .OrderByDescending(p => p.FechaPedido)
                    .ToListAsync()
            };

            return View(dashboard);
        }
    }
}