using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

[Authorize(Roles = "Administrador, Cajero")]
public class PlatillosController : Controller
{
    private readonly AppDbContext _context;

    public PlatillosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: PLATILLOS
    public async Task<IActionResult> Index()
    {
        var platillos = _context.Platillos.Include(p => p.CategoriaPlato);
        return View(await platillos.ToListAsync());
    }

    // GET: PLATILLOS/Details/5
    public async Task<IActionResult> Details(int? id) // <-- Cambiado a id
    {
        if (id == null)
        {
            return NotFound();
        }

        var platillo = await _context.Platillos
            .Include(p => p.CategoriaPlato) // Agregamos el include para ver el texto de la categoría
            .FirstOrDefaultAsync(m => m.IdPlatillo == id);

        if (platillo == null)
        {
            return NotFound();
        }

        return View(platillo);
    }

    // GET: PLATILLOS/Create
    public IActionResult Create()
    {
        ViewBag.Categorias = new SelectList(_context.CategoriasPlatos, "IdCategoria", "NombreCategoria");
        return View();
    }

    // POST: PLATILLOS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPlatillo,NombrePlatillo,DescripcionGourmet,PrecioVenta,IdCategoria,TiempoEstimadoMin,Disponibilidad")] Platillo platillo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(platillo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(_context.CategoriasPlatos, "IdCategoria", "NombreCategoria", platillo.IdCategoria);
        return View(platillo);
    }

    // GET: PLATILLOS/Edit/5
    public async Task<IActionResult> Edit(int? id) // <-- Cambiado a id
    {
        if (id == null)
        {
            return NotFound();
        }

        var platillo = await _context.Platillos.FindAsync(id);
        if (platillo == null)
        {
            return NotFound();
        }

        // Cargamos las categorías para el combobox
        ViewBag.Categorias = new SelectList(_context.CategoriasPlatos, "IdCategoria", "NombreCategoria", platillo.IdCategoria);
        return View(platillo);
    }

    // POST: PLATILLOS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdPlatillo,NombrePlatillo,DescripcionGourmet,PrecioVenta,IdCategoria,TiempoEstimadoMin,Disponibilidad")] Platillo platillo)
    {
        if (id != platillo.IdPlatillo)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(platillo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatilloExists(platillo.IdPlatillo)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(_context.CategoriasPlatos, "IdCategoria", "NombreCategoria", platillo.IdCategoria);
        return View(platillo);
    }

    // GET: PLATILLOS/Delete/5
    public async Task<IActionResult> Delete(int? id) // <-- Cambiado a id
    {
        if (id == null)
        {
            return NotFound();
        }

        var platillo = await _context.Platillos
            .Include(p => p.CategoriaPlato)
            .FirstOrDefaultAsync(m => m.IdPlatillo == id);

        if (platillo == null)
        {
            return NotFound();
        }

        return View(platillo);
    }

    // POST: PLATILLOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) // <-- Cambiado a id
    {
        var platillo = await _context.Platillos.FindAsync(id);
        if (platillo != null)
        {
            _context.Platillos.Remove(platillo);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PlatilloExists(int id)
    {
        return _context.Platillos.Any(e => e.IdPlatillo == id);
    }
}