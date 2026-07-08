
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

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
        return View(await _context.Reservas.ToListAsync());
    }

    // GET: RESERVAS/Details/5
    public async Task<IActionResult> Details(long? idreserva)
    {
        if (idreserva == null)
        {
            return NotFound();
        }

        var reserva = await _context.Reservas
         .Include(r => r.Cliente)
         .Include(r => r.Mesa)
         .FirstOrDefaultAsync(r => r.IdReserva == idreserva);
        if (reserva == null)
        {
            return NotFound();
        }

        return View(reserva);
    }

    // GET: RESERVAS/Create
    public IActionResult Create()
    {
        ViewBag.Clientes = new SelectList(
            _context.Clientes
            .Select(c => new {
                c.IdCliente,
                Nombre = c.Nombre + " " + c.Apellido
            }),
            "IdCliente",
            "Nombre"
        );

        ViewBag.Mesas = new SelectList(
            _context.Mesas
            .Select(m => new {
                m.IdMesa,
                Texto = "Mesa " + m.NumeroMesa + " - " + m.Capacidad + " personas"
            }),
            "IdMesa",
            "Texto"
        );

        return View();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
     [Bind("IdCliente,IdMesa,FechaReserva,HoraReserva,CantidadPersonas,Origen")] Reserva reserva)
    {
        var mesa = await _context.Mesas
    .FirstOrDefaultAsync(m => m.IdMesa == reserva.IdMesa);

        if (mesa != null && reserva.CantidadPersonas > mesa.Capacidad)
        {
            ModelState.AddModelError("CantidadPersonas",
                $"La mesa seleccionada solo tiene capacidad para {mesa.Capacidad} personas.");
        }
        if (reserva.FechaReserva.Date < DateTime.Today)
        {
            ModelState.AddModelError("FechaReserva",
                "No puede registrar reservas en fechas pasadas.");
        }
        var horaMinima = new TimeSpan(9, 0, 0);
        var horaMaxima = new TimeSpan(22, 0, 0);

        if (reserva.HoraReserva < horaMinima ||
            reserva.HoraReserva > horaMaxima)
        {
            ModelState.AddModelError("HoraReserva",
                "Las reservas solo pueden realizarse entre las 09:00 AM y las 10:00 PM.");
        }
        if (ModelState.IsValid)
        {
            reserva.EstadoReserva = "Pendiente";
            reserva.FechaCreacion = DateTime.Now;

            _context.Add(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // RECARGAR DROPDOWNS SI FALLA VALIDACIÓN
        ViewBag.Clientes = new SelectList(
            _context.Clientes
            .Select(c => new {
                c.IdCliente,
                Nombre = c.Nombre + " " + c.Apellido
            }),
            "IdCliente",
            "Nombre"
        );

        ViewBag.Mesas = new SelectList(
            _context.Mesas
            .Select(m => new {
                m.IdMesa,
                Texto = "Mesa " + m.NumeroMesa + " - " + m.Capacidad + " personas"
            }),
            "IdMesa",
            "Texto"
        );

        return View(reserva);
    }
    // GET: RESERVAS/Edit/5
    public async Task<IActionResult> Edit(long? idreserva)
    {
        if (idreserva == null)
        {
            return NotFound();
        }

        var reserva = await _context.Reservas.FindAsync(idreserva);

        if (reserva == null)
        {
            return NotFound();
        }
        ViewBag.Clientes = new SelectList(
            _context.Clientes.Select(c => new
            {
                c.IdCliente,
                Nombre = c.Nombre + " " + c.Apellido
            }),
            "IdCliente",
            "Nombre",
            reserva.IdCliente
        );

        ViewBag.Mesas = new SelectList(
            _context.Mesas.Select(m => new
            {
                m.IdMesa,
                Texto = "Mesa " + m.NumeroMesa
            }),
            "IdMesa",
            "Texto",
            reserva.IdMesa
        );


        return View(reserva);
    }

    // POST: RESERVAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        long? idreserva,
        [Bind("IdReserva,IdCliente,IdMesa,FechaReserva,HoraReserva,CantidadPersonas,Origen,EstadoReserva")]
    Reserva reserva)
    {
        if (idreserva != reserva.IdReserva)
        {
            return NotFound();
        }

        // Validación de capacidad de mesa
        var mesa = await _context.Mesas.FindAsync(reserva.IdMesa);

        if (mesa != null && reserva.CantidadPersonas > mesa.Capacidad)
        {
            ModelState.AddModelError(
                "CantidadPersonas",
                $"La mesa seleccionada solo admite {mesa.Capacidad} personas.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(reserva.IdReserva))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // RECARGAR COMBOS
        ViewBag.Clientes = new SelectList(
            _context.Clientes.Select(c => new
            {
                c.IdCliente,
                Nombre = c.Nombre + " " + c.Apellido
            }),
            "IdCliente",
            "Nombre",
            reserva.IdCliente
        );

        ViewBag.Mesas = new SelectList(
            _context.Mesas.Select(m => new
            {
                m.IdMesa,
                Texto = "Mesa " + m.NumeroMesa + " - " + m.Capacidad + " personas"
            }),
            "IdMesa",
            "Texto",
            reserva.IdMesa
        );

        return View(reserva);
    }

    // GET: RESERVAS/Delete/5
    public async Task<IActionResult> Delete(long? idreserva)
    {
        if (idreserva == null)
        {
            return NotFound();
        }

        var reserva = await _context.Reservas
            .FirstOrDefaultAsync(m => m.IdReserva == idreserva);
        if (reserva == null)
        {
            return NotFound();
        }

        return View(reserva);
    }

    // POST: RESERVAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long? idreserva)
    {
        var reserva = await _context.Reservas
            .Include(r => r.Cliente)
            .Include(r => r.Mesa)
            .FirstOrDefaultAsync(r => r.IdReserva == idreserva);
        {
            _context.Reservas.Remove(reserva);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ReservaExists(long? idreserva)
    {
        return _context.Reservas.Any(e => e.IdReserva == idreserva);
    }
}
