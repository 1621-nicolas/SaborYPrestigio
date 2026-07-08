
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;
using SaborPrestigioMVC.Data;

[Authorize]
public class RolController : Controller
{
    private readonly AppDbContext _context;

    public RolController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ROLS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Roles.ToListAsync());
    }

    // GET: ROLS/Details/5
    public async Task<IActionResult> Details(int? idrol)
    {
        if (idrol == null)
        {
            return NotFound();
        }

        var rol = await _context.Roles
            .FirstOrDefaultAsync(m => m.IdRol == idrol);
        if (rol == null)
        {
            return NotFound();
        }

        return View(rol);
    }

    // GET: ROLS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ROLS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdRol,NombreRol,Descripcion,Empleados")] Rol rol)
    {
        if (ModelState.IsValid)
        {
            _context.Add(rol);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(rol);
    }

    // GET: ROLS/Edit/5
    public async Task<IActionResult> Edit(int? idrol)
    {
        if (idrol == null)
        {
            return NotFound();
        }

        var rol = await _context.Roles.FindAsync(idrol);
        if (rol == null)
        {
            return NotFound();
        }
        return View(rol);
    }

    // POST: ROLS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idrol, [Bind("IdRol,NombreRol,Descripcion,Empleados")] Rol rol)
    {
        if (idrol != rol.IdRol)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(rol);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(rol.IdRol))
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
        return View(rol);
    }

    // GET: ROLS/Delete/5
    public async Task<IActionResult> Delete(int? idrol)
    {
        if (idrol == null)
        {
            return NotFound();
        }

        var rol = await _context.Roles
            .FirstOrDefaultAsync(m => m.IdRol == idrol);
        if (rol == null)
        {
            return NotFound();
        }

        return View(rol);
    }

    // POST: ROLS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idrol)
    {
        var rol = await _context.Roles.FindAsync(idrol);
        if (rol != null)
        {
            _context.Roles.Remove(rol);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool RolExists(int? idrol)
    {
        return _context.Roles.Any(e => e.IdRol == idrol);
    }
}
