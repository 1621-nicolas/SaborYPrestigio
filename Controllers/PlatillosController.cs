
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;


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
        var platillos = _context.Platillos
            .Include(p => p.CategoriaPlato);

        return View(await platillos.ToListAsync());
    }

    // GET: PLATILLOS/Details/5
    public async Task<IActionResult> Details(int? idplatillo)
    {
        if (idplatillo == null)
        {
            return NotFound();
        }

        var platillo = await _context.Platillos
            .FirstOrDefaultAsync(m => m.IdPlatillo == idplatillo);
        if (platillo == null)
        {
            return NotFound();
        }

        return View(platillo);
    }

    // GET: PLATILLOS/Create
    public IActionResult Create()
    {
        ViewBag.Categorias = new SelectList(
            _context.CategoriasPlatos,
            "IdCategoria",
            "NombreCategoria"
        );

        return View();
    }

    // POST: PLATILLOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPlatillo,NombrePlatillo,DescripcionGourmet,PrecioVenta,IdCategoria,TiempoEstimadoMin,Disponibilidad,CategoriaPlato,DetallePedidos,RecetasPlatillo")] Platillo platillo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(platillo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(platillo);
    }

    // GET: PLATILLOS/Edit/5
    public async Task<IActionResult> Edit(int? idplatillo)
    {
        if (idplatillo == null)
        {
            return NotFound();
        }

        var platillo = await _context.Platillos.FindAsync(idplatillo);
        if (platillo == null)
        {
            return NotFound();
        }
        return View(platillo);
    }

    // POST: PLATILLOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idplatillo, [Bind("IdPlatillo,NombrePlatillo,DescripcionGourmet,PrecioVenta,IdCategoria,TiempoEstimadoMin,Disponibilidad")] Platillo platillo)
    {
        if (idplatillo != platillo.IdPlatillo)
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
                if (!PlatilloExists(platillo.IdPlatillo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(
            _context.CategoriasPlatos,
            "IdCategoria",
            "NombreCategoria",
            platillo.IdCategoria
        );

        return View(platillo);
    }
    // GET: PLATILLOS/Delete/5
    public async Task<IActionResult> Delete(int? idplatillo)
    {
        if (idplatillo == null)
        {
            return NotFound();
        }

        var platillo = await _context.Platillos
            .FirstOrDefaultAsync(m => m.IdPlatillo == idplatillo);
        if (platillo == null)
        {
            return NotFound();
        }

        return View(platillo);
    }

    // POST: PLATILLOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idplatillo)
    {
        var platillo = await _context.Platillos.FindAsync(idplatillo);
        if (platillo != null)
        {
            _context.Platillos.Remove(platillo);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PlatilloExists(int? idplatillo)
    {
        return _context.Platillos.Any(e => e.IdPlatillo == idplatillo);
    }
}
