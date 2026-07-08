using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalPedidos = await _context.Pedidos.CountAsync(),
                TotalReservas = await _context.Reservas.CountAsync(),
                MesasDisponibles = await _context.Mesas.CountAsync(m => m.Estado == "Disponible"),
                PlatillosDisponibles = await _context.Platillos.CountAsync(p => p.Disponibilidad),
                StockCritico = await _context.Insumos.CountAsync(i => i.StockActual <= i.StockMinimo),
                ComprobantesEmitidos = await _context.ComprobantesPago.CountAsync(),
                VentasTotales = await _context.Pedidos.SumAsync(p => p.Total),


                UltimosPedidos = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .OrderByDescending(p => p.IdPedido)
                    .Take(5)
                    .ToListAsync(),
                VentasPorDia = await _context.Pedidos
    .GroupBy(p => p.FechaPedido.Date)
    .Select(g => new VentaPorDiaViewModel
    {
        Dia = g.Key.ToString("dd/MM"),
        Total = g.Sum(p => p.Total)
    })
    .ToListAsync()
            };

            return View(dashboard);
        }
    }
}
