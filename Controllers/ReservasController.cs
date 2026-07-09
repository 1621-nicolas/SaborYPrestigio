using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RESERVAS
        public async Task<IActionResult> Index()
        {
            // 🔥 AQUÍ ESTÁ LA CORRECCIÓN: Incluimos los datos relacionados
            var reservas = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();

            return View(reservas);
        }

        // GET: RESERVAS/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var reserva = await _context.Reservas
             .Include(r => r.Cliente)
             .Include(r => r.Mesa)
             .FirstOrDefaultAsync(r => r.IdReserva == id);

            if (reserva == null) return NotFound();

            return View(reserva);
        }

        // GET: Reservas/Create
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Cliente"))
            {
                var claimId = User.FindFirst("UsuarioId")?.Value;
                long idCliente = string.IsNullOrEmpty(claimId) ? 0 : long.Parse(claimId);
                var clienteInfo = await _context.Clientes.FindAsync(idCliente);

                ViewBag.EsClienteWeb = true;
                ViewBag.IdClienteLogueado = idCliente;
                ViewBag.NombreClienteLogueado = clienteInfo != null ? $"{clienteInfo.Nombre} {clienteInfo.Apellido}".Trim() : "Cliente Web";
            }
            else
            {
                ViewBag.EsClienteWeb = false;
                ViewBag.Clientes = new SelectList(_context.Clientes.Select(c => new { c.IdCliente, Nombre = c.Nombre + " " + c.Apellido }), "IdCliente", "Nombre");
            }

            ViewBag.Mesas = new SelectList(_context.Mesas, "IdMesa", "NumeroMesa");
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReserva,IdCliente,IdMesa,FechaReserva,HoraReserva,CantidadPersonas,Origen")] Reserva reserva)
        {
            if (User.IsInRole("Cliente"))
            {
                var claimId = User.FindFirst("UsuarioId")?.Value;
                reserva.IdCliente = string.IsNullOrEmpty(claimId) ? 0 : long.Parse(claimId);
                reserva.Origen = "Web";
                reserva.IdMesa = null;
            }

            if (ModelState.IsValid)
            {
                reserva.EstadoReserva = "Pendiente";
                reserva.FechaCreacion = DateTime.Now;
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return User.IsInRole("Cliente") ? RedirectToAction("Index", "ClientePortal") : RedirectToAction(nameof(Index));
            }

            ViewBag.Mesas = new SelectList(_context.Mesas, "IdMesa", "NumeroMesa", reserva.IdMesa);
            return View(reserva);
        }

        // GET: RESERVAS/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            ViewBag.Mesas = new SelectList(_context.Mesas, "IdMesa", "NumeroMesa", reserva.IdMesa);
            ViewBag.Clientes = new SelectList(_context.Clientes.Select(c => new { c.IdCliente, Nombre = c.Nombre + " " + c.Apellido }), "IdCliente", "Nombre", reserva.IdCliente);
            return View(reserva);
        }

        // POST: RESERVAS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("IdReserva,IdCliente,IdMesa,FechaReserva,HoraReserva,CantidadPersonas,Origen,EstadoReserva,FechaCreacion")] Reserva reserva)
        {
            if (id != reserva.IdReserva) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reserva);
        }

        // GET: RESERVAS/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();
            var reserva = await _context.Reservas.Include(r => r.Cliente).Include(r => r.Mesa).FirstOrDefaultAsync(m => m.IdReserva == id);
            return reserva == null ? NotFound() : View(reserva);
        }

        // POST: RESERVAS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null) { _context.Reservas.Remove(reserva); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}