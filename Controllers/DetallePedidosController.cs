
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;
using SaborPrestigioMVC.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
public class DetallePedidosController : Controller
{
    private readonly AppDbContext _context;

    public DetallePedidosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: DETALLEPEDIDOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.DetallePedidos.ToListAsync());
    }

    // GET: DETALLEPEDIDOS/Details/5
    public async Task<IActionResult> Details(long? iddetalle)
    {
        if (iddetalle == null)
        {
            return NotFound();
        }

        var detallepedido = await _context.DetallePedidos
            .FirstOrDefaultAsync(m => m.IdDetalle == iddetalle);
        if (detallepedido == null)
        {
            return NotFound();
        }

        return View(detallepedido);
    }

    // GET: DETALLEPEDIDOS/Create
    public IActionResult Create()
    {
        ViewBag.Pedidos = new SelectList(
            _context.Pedidos,
            "IdPedido",
            "IdPedido"
        );

        ViewBag.Platillos = new SelectList(
            _context.Platillos,
            "IdPlatillo",
            "NombrePlatillo"
        );

        return View();
    }

    // POST: DETALLEPEDIDOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPedido,IdPlatillo,Cantidad,PrecioUnitario,NotasChef")] DetallePedido detallepedido)
    {
        if (ModelState.IsValid)
        {
            _context.Add(detallepedido);
            await _context.SaveChangesAsync();

            var pedido = await _context.Pedidos.FindAsync(detallepedido.IdPedido);

            if (pedido != null)
            {
                pedido.Total = _context.DetallePedidos
                    .Where(d => d.IdPedido == detallepedido.IdPedido)
                    .Sum(d => d.Cantidad * d.PrecioUnitario);

                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Pedidos = new SelectList(_context.Pedidos, "IdPedido", "IdPedido", detallepedido.IdPedido);
        ViewBag.Platillos = new SelectList(_context.Platillos, "IdPlatillo", "NombrePlatillo", detallepedido.IdPlatillo);

        return View(detallepedido);
    }

    // GET: DETALLEPEDIDOS/Edit/5
    public async Task<IActionResult> Edit(long? iddetalle)
    {
        if (iddetalle == null)
        {
            return NotFound();
        }

        var detallepedido = await _context.DetallePedidos.FindAsync(iddetalle);
        if (detallepedido == null)
        {
            return NotFound();
        }
        return View(detallepedido);
    }

    // POST: DETALLEPEDIDOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long? iddetalle, [Bind("IdDetalle,IdPedido,IdPlatillo,Cantidad,PrecioUnitario,Subtotal,NotasChef,Pedido,Platillo")] DetallePedido detallepedido)
    {
        if (iddetalle != detallepedido.IdDetalle)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(detallepedido);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetallePedidoExists(detallepedido.IdDetalle))
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
        return View(detallepedido);
    }

    // GET: DETALLEPEDIDOS/Delete/5
    public async Task<IActionResult> Delete(long? iddetalle)
    {
        if (iddetalle == null)
        {
            return NotFound();
        }

        var detallepedido = await _context.DetallePedidos
            .FirstOrDefaultAsync(m => m.IdDetalle == iddetalle);
        if (detallepedido == null)
        {
            return NotFound();
        }

        return View(detallepedido);
    }

    // POST: DETALLEPEDIDOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long? iddetalle)
    {
        var detallepedido = await _context.DetallePedidos.FindAsync(iddetalle);
        if (detallepedido != null)
        {
            _context.DetallePedidos.Remove(detallepedido);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DetallePedidoExists(long? iddetalle)
    {
        return _context.DetallePedidos.Any(e => e.IdDetalle == iddetalle);
    }
}
