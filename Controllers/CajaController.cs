using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Controllers
{
    [Authorize]
    public class CajaController : Controller
    {
        private readonly AppDbContext _context;

        public CajaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;

            var comprobantesHoy = _context.ComprobantesPago
                .Include(c => c.Pedido)
                .Where(c => c.FechaEmision.Date == hoy);

            var dashboard = new CajaDashboardViewModel
            {
                PedidosPorCobrar = await _context.Pedidos
                    .CountAsync(p => p.EstadoPedido == "Listo"
                                  || p.EstadoPedido == "Pendiente de Pago"),

                ComprobantesEmitidos = await comprobantesHoy.CountAsync(),

                VentasDelDia = await comprobantesHoy.SumAsync(c => (decimal?)c.MontoTotal) ?? 0,

                IgvDelDia = await comprobantesHoy.SumAsync(c => (decimal?)c.MontoIgv) ?? 0,

                PedidosPendientesPago = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Mesa)
                    .Where(p => p.EstadoPedido == "Listo"
                             || p.EstadoPedido == "Pendiente de Pago")
                    .OrderByDescending(p => p.FechaPedido)
                    .ToListAsync(),

                ComprobantesDelDia = await comprobantesHoy
                    .OrderByDescending(c => c.FechaEmision)
                    .ToListAsync(),

                MetodosPago = await comprobantesHoy
                    .GroupBy(c => c.MetodoPago)
                    .Select(g => new MetodoPagoViewModel
                    {
                        MetodoPago = g.Key,
                        Total = g.Sum(x => x.MontoTotal),
                        Cantidad = g.Count()
                    })
                    .ToListAsync()
            };

            return View(dashboard);
        }
    }
}