
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;
using SaborPrestigioMVC.Data;

[Authorize]
public class CategoriaPlatosController : Controller
{
    private readonly AppDbContext _context;

    public CategoriaPlatosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: CATEGORIAPLATOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.CategoriasPlatos.ToListAsync());
    }

    // GET: CATEGORIAPLATOS/Details/5
    public async Task<IActionResult> Details(int? idcategoria)
    {
        if (idcategoria == null)
        {
            return NotFound();
        }

        var categoriaplato = await _context.CategoriasPlatos
            .FirstOrDefaultAsync(m => m.IdCategoria == idcategoria);
        if (categoriaplato == null)
        {
            return NotFound();
        }

        return View(categoriaplato);
    }

    // GET: CATEGORIAPLATOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CATEGORIAPLATOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdCategoria,NombreCategoria,Descripcion,Platillos")] CategoriaPlato categoriaplato)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categoriaplato);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(categoriaplato);
    }

    // GET: CATEGORIAPLATOS/Edit/5
    public async Task<IActionResult> Edit(int? idcategoria)
    {
        if (idcategoria == null)
        {
            return NotFound();
        }

        var categoriaplato = await _context.CategoriasPlatos.FindAsync(idcategoria);
        if (categoriaplato == null)
        {
            return NotFound();
        }
        return View(categoriaplato);
    }

    // POST: CATEGORIAPLATOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idcategoria, [Bind("IdCategoria,NombreCategoria,Descripcion,Platillos")] CategoriaPlato categoriaplato)
    {
        if (idcategoria != categoriaplato.IdCategoria)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(categoriaplato);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaPlatoExists(categoriaplato.IdCategoria))
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
        return View(categoriaplato);
    }

    // GET: CATEGORIAPLATOS/Delete/5
    public async Task<IActionResult> Delete(int? idcategoria)
    {
        if (idcategoria == null)
        {
            return NotFound();
        }

        var categoriaplato = await _context.CategoriasPlatos
            .FirstOrDefaultAsync(m => m.IdCategoria == idcategoria);
        if (categoriaplato == null)
        {
            return NotFound();
        }

        return View(categoriaplato);
    }

    // POST: CATEGORIAPLATOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idcategoria)
    {
        var categoriaplato = await _context.CategoriasPlatos.FindAsync(idcategoria);
        if (categoriaplato != null)
        {
            _context.CategoriasPlatos.Remove(categoriaplato);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoriaPlatoExists(int? idcategoria)
    {
        return _context.CategoriasPlatos.Any(e => e.IdCategoria == idcategoria);
    }
}
