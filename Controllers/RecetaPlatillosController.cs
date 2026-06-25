using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

public class RecetaPlatillosController : Controller
{
    private readonly AppDbContext _context;

    public RecetaPlatillosController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var recetas = _context.RecetasPlatillo
            .Include(r => r.Platillo)
            .Include(r => r.Insumo);

        return View(await recetas.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Platillos = new SelectList(_context.Platillos, "IdPlatillo", "NombrePlatillo");
        ViewBag.Insumos = new SelectList(_context.Insumos, "IdInsumo", "NombreInsumo");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPlatillo,IdInsumo,CantidadRequerida")] RecetaPlatillo receta)
    {
        if (ModelState.IsValid)
        {
            _context.Add(receta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Platillos = new SelectList(_context.Platillos, "IdPlatillo", "NombrePlatillo", receta.IdPlatillo);
        ViewBag.Insumos = new SelectList(_context.Insumos, "IdInsumo", "NombreInsumo", receta.IdInsumo);

        return View(receta);
    }
}