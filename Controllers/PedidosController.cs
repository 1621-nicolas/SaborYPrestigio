
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;


public class PedidosController : Controller
{
    private readonly AppDbContext _context;

    public PedidosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: PEDIDOS
    public async Task<IActionResult> Index()
    {
        var pedidos = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Empleado)
            .Include(p => p.Mesa);

        return View(await pedidos.ToListAsync());
    }

    // GET: PEDIDOS/Details/5
    public async Task<IActionResult> Details(long? idpedido)
    {
        if (idpedido == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(m => m.IdPedido == idpedido);
        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    // GET: PEDIDOS/Create
    public IActionResult Create()
    {
        ViewBag.Clientes = new SelectList(
            _context.Clientes,
            "IdCliente",
            "Nombre"
        );

        ViewBag.Mesas = new SelectList(
            _context.Mesas,
            "IdMesa",
            "NumeroMesa"
        );

        ViewBag.Empleados = new SelectList(
            _context.Empleados,
            "IdEmpleado",
            "Nombre"
        );

        return View();
    }

    // POST: PEDIDOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPedido,IdCliente,IdEmpleado,IdMesa,TipoPedido,FechaPedido,EstadoPedido,Total,Cliente,Empleado,Mesa,DetallePedidos,ComprobantePago")] Pedido pedido)
    {
        if (ModelState.IsValid)
        {
            _context.Add(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(pedido);
    }

    // GET: PEDIDOS/Edit/5
    public async Task<IActionResult> Edit(long? idpedido)
    {
        if (idpedido == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos.FindAsync(idpedido);
        if (pedido == null)
        {
            return NotFound();
        }
        return View(pedido);
    }

    // POST: PEDIDOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long? idpedido, [Bind("IdPedido,IdCliente,IdEmpleado,IdMesa,TipoPedido,FechaPedido,EstadoPedido,Total,Cliente,Empleado,Mesa,DetallePedidos,ComprobantePago")] Pedido pedido)
    {
        if (idpedido != pedido.IdPedido)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(pedido.IdPedido))
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
        return View(pedido);
    }

    // GET: PEDIDOS/Delete/5
    public async Task<IActionResult> Delete(long? idpedido)
    {
        if (idpedido == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(m => m.IdPedido == idpedido);
        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    // POST: PEDIDOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long? idpedido)
    {
        var pedido = await _context.Pedidos.FindAsync(idpedido);
        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PedidoExists(long? idpedido)
    {
        return _context.Pedidos.Any(e => e.IdPedido == idpedido);
    }
}
