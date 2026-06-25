
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;
using SaborPrestigioMVC.Data;

public class InsumosController : Controller
{
    private readonly AppDbContext _context;

    public InsumosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: INSUMOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Insumos.ToListAsync());
    }

    // GET: INSUMOS/Details/5
    public async Task<IActionResult> Details(int? idinsumo)
    {
        if (idinsumo == null)
        {
            return NotFound();
        }

        var insumo = await _context.Insumos
            .FirstOrDefaultAsync(m => m.IdInsumo == idinsumo);
        if (insumo == null)
        {
            return NotFound();
        }

        return View(insumo);
    }

    // GET: INSUMOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: INSUMOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdInsumo,NombreInsumo,UnidadMedida,StockActual,StockMinimo,PrecioCostoPromedio,RecetasPlatillo")] Insumo insumo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(insumo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(insumo);
    }

    // GET: INSUMOS/Edit/5
    public async Task<IActionResult> Edit(int? idinsumo)
    {
        if (idinsumo == null)
        {
            return NotFound();
        }

        var insumo = await _context.Insumos.FindAsync(idinsumo);
        if (insumo == null)
        {
            return NotFound();
        }
        return View(insumo);
    }

    // POST: INSUMOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idinsumo, [Bind("IdInsumo,NombreInsumo,UnidadMedida,StockActual,StockMinimo,PrecioCostoPromedio,RecetasPlatillo")] Insumo insumo)
    {
        if (idinsumo != insumo.IdInsumo)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(insumo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InsumoExists(insumo.IdInsumo))
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
        return View(insumo);
    }

    // GET: INSUMOS/Delete/5
    public async Task<IActionResult> Delete(int? idinsumo)
    {
        if (idinsumo == null)
        {
            return NotFound();
        }

        var insumo = await _context.Insumos
            .FirstOrDefaultAsync(m => m.IdInsumo == idinsumo);
        if (insumo == null)
        {
            return NotFound();
        }

        return View(insumo);
    }

    // POST: INSUMOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idinsumo)
    {
        var insumo = await _context.Insumos.FindAsync(idinsumo);
        if (insumo != null)
        {
            _context.Insumos.Remove(insumo);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool InsumoExists(int? idinsumo)
    {
        return _context.Insumos.Any(e => e.IdInsumo == idinsumo);
    }
}
