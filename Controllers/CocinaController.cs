using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Controllers
{
    public class CocinaController : Controller
    {
        private readonly AppDbContext _context;

        public CocinaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new CocinaDashboardViewModel
            {
                PedidosEnEspera = await _context.Pedidos.CountAsync(p => p.EstadoPedido == "En Espera"),
                PedidosEnCocina = await _context.Pedidos.CountAsync(p => p.EstadoPedido == "En Preparación"),
                PedidosListos = await _context.Pedidos.CountAsync(p => p.EstadoPedido == "Listo"),

                PedidosCocina = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Mesa)
                    .Include(p => p.DetallePedidos)
                        .ThenInclude(d => d.Platillo)
                    .Where(p => p.EstadoPedido == "En Espera"
                             || p.EstadoPedido == "En Preparación"
                             || p.EstadoPedido == "Listo")
                    .OrderBy(p => p.FechaPedido)
                    .ToListAsync(),

                PlatillosMasPedidos = await _context.DetallePedidos
                    .Include(d => d.Platillo)
                    .GroupBy(d => d.Platillo.NombrePlatillo)
                    .Select(g => new PlatilloMasPedidoViewModel
                    {
                        NombrePlatillo = g.Key,
                        CantidadVendida = g.Sum(x => x.Cantidad)
                    })
                    .OrderByDescending(x => x.CantidadVendida)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(long idPedido, string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(idPedido);

            if (pedido == null)
            {
                return NotFound();
            }

            pedido.EstadoPedido = nuevoEstado;

            _context.Update(pedido);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}