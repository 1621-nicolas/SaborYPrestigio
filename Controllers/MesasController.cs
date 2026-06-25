
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;
using SaborPrestigioMVC.Data;

public class MesasController : Controller
{
    private readonly AppDbContext _context;

    public MesasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: MESAS
    public async Task<IActionResult> Index()
    {
        return View(await _context.Mesas
            .Include(m => m.Reservas)
            .Include(m => m.Pedidos)
            .ToListAsync());
    }
    // GET: MESAS/Details/5
    public async Task<IActionResult> Details(int? idmesa)
    {
        if (idmesa == null)
        {
            return NotFound();
        }

        var mesa = await _context.Mesas
            .FirstOrDefaultAsync(m => m.IdMesa == idmesa);
        if (mesa == null)
        {
            return NotFound();
        }

        return View(mesa);
    }

    // GET: MESAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MESAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NumeroMesa,Capacidad,Zona,Estado")] Mesa mesa)
    {
        if (_context.Mesas.Any(m => m.NumeroMesa == mesa.NumeroMesa))
        {
            ModelState.AddModelError("NumeroMesa",
                "Ya existe una mesa con ese número.");
        }
        if (ModelState.IsValid)
        {
            _context.Add(mesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(mesa);
    }

    // GET: MESAS/Edit/5
    public async Task<IActionResult> Edit(int? idmesa)
    {
        if (idmesa == null)
        {
            return NotFound();
        }

        var mesa = await _context.Mesas.FindAsync(idmesa);
        if (mesa == null)
        {
            return NotFound();
        }
        return View(mesa);
    }

    // POST: MESAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
     int? idmesa,
     [Bind("IdMesa,NumeroMesa,Capacidad,Zona,Estado")] Mesa mesa)
    {
        if (idmesa != mesa.IdMesa)
        {
            return NotFound();
        }
        if (_context.Mesas.Any(m =>
            m.NumeroMesa == mesa.NumeroMesa &&
            m.IdMesa != mesa.IdMesa))
        {
            ModelState.AddModelError(
                "NumeroMesa",
                "Ya existe una mesa con ese número.");
        }
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(mesa);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MesaExists(mesa.IdMesa))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(mesa);
    }

    // GET: MESAS/Delete/5
    public async Task<IActionResult> Delete(int? idmesa)
    {
        if (idmesa == null)
        {
            return NotFound();
        }

        var mesa = await _context.Mesas
            .FirstOrDefaultAsync(m => m.IdMesa == idmesa);
        if (mesa == null)
        {
            return NotFound();
        }

        return View(mesa);
    }

    // POST: MESAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int IdMesa)
    {
        var mesa = await _context.Mesas.FindAsync(IdMesa);

        if (mesa != null)
        {
            mesa.Estado = "Fuera de Servicio";

            _context.Update(mesa);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
    private bool MesaExists(int? idmesa)
    {
        return _context.Mesas.Any(e => e.IdMesa == idmesa);
    }
}
