
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;
using SaborPrestigioMVC.Data;

[Authorize]
public class EmpleadosController : Controller
{
    private readonly AppDbContext _context;

    public EmpleadosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: EMPLEADOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Empleados.ToListAsync());
    }

    // GET: EMPLEADOS/Details/5
    public async Task<IActionResult> Details(int? idempleado)
    {
        if (idempleado == null)
        {
            return NotFound();
        }

        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(m => m.IdEmpleado == idempleado);
        if (empleado == null)
        {
            return NotFound();
        }

        return View(empleado);
    }

    // GET: EMPLEADOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: EMPLEADOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdEmpleado,Dni,Nombre,Apellido,Telefono,Correo,Usuario,ContraseniaHash,IdRol,Estado,Rol,Pedidos")] Empleado empleado)
    {
        if (ModelState.IsValid)
        {
            _context.Add(empleado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(empleado);
    }

    // GET: EMPLEADOS/Edit/5
    public async Task<IActionResult> Edit(int? idempleado)
    {
        if (idempleado == null)
        {
            return NotFound();
        }

        var empleado = await _context.Empleados.FindAsync(idempleado);
        if (empleado == null)
        {
            return NotFound();
        }
        return View(empleado);
    }

    // POST: EMPLEADOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idempleado, [Bind("IdEmpleado,Dni,Nombre,Apellido,Telefono,Correo,Usuario,ContraseniaHash,IdRol,Estado,Rol,Pedidos")] Empleado empleado)
    {
        if (idempleado != empleado.IdEmpleado)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(empleado);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(empleado.IdEmpleado))
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
        return View(empleado);
    }

    // GET: EMPLEADOS/Delete/5
    public async Task<IActionResult> Delete(int? idempleado)
    {
        if (idempleado == null)
        {
            return NotFound();
        }

        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(m => m.IdEmpleado == idempleado);
        if (empleado == null)
        {
            return NotFound();
        }

        return View(empleado);
    }

    // POST: EMPLEADOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idempleado)
    {
        var empleado = await _context.Empleados.FindAsync(idempleado);
        if (empleado != null)
        {
            _context.Empleados.Remove(empleado);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EmpleadoExists(int? idempleado)
    {
        return _context.Empleados.Any(e => e.IdEmpleado == idempleado);
    }
}
