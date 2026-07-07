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

    // GET: RecetaPlatillos
    public async Task<IActionResult> Index()
    {
        var recetas = _context.RecetasPlatillo
            .Include(r => r.Platillo)
            .Include(r => r.Insumo)
            .OrderBy(r => r.Platillo!.NombrePlatillo); // Agrupamos visualmente por platillo

        return View(await recetas.ToListAsync());
    }

    // GET: RecetaPlatillos/Create
    public IActionResult Create()
    {
        ViewBag.Platillos = new SelectList(_context.Platillos, "IdPlatillo", "NombrePlatillo");

        // Enviamos los insumos con su unidad de medida para que se vea genial en JS
        ViewBag.InsumosRaw = _context.Insumos
            .Select(i => new {
                IdInsumo = i.IdInsumo,
                NombreUnidad = $"{i.NombreInsumo} ({i.UnidadMedida})"
            }).ToList();

        return View();
    }

    // POST: RecetaPlatillos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int idPlatillo, List<RecetaPlatillo> ingredientes)
    {
        if (ingredientes == null || !ingredientes.Any())
        {
            ModelState.AddModelError("", "Debe añadir al menos un insumo a la receta.");
        }

        if (ModelState.IsValid)
        {
            // Opcional: Eliminar ingredientes anteriores si se está sobreescribiendo la receta
            var anteriores = _context.RecetasPlatillo.Where(r => r.IdPlatillo == idPlatillo);
            _context.RecetasPlatillo.RemoveRange(anteriores);

            foreach (var item in ingredientes)
            {
                item.IdPlatillo = idPlatillo;
                _context.Add(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Platillos = new SelectList(_context.Platillos, "IdPlatillo", "NombrePlatillo", idPlatillo);
        ViewBag.InsumosRaw = _context.Insumos.Select(i => new { IdInsumo = i.IdInsumo, NombreUnidad = $"{i.NombreInsumo} ({i.UnidadMedida})" }).ToList();
        return View();
    }
}