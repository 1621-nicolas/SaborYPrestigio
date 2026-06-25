using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

public class ComprobantedePagoController : Controller
{
    private readonly AppDbContext _context;

    public ComprobantedePagoController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var comprobantes = _context.ComprobantesPago
            .Include(c => c.Pedido)
            .ThenInclude(p => p.Cliente);

        return View(await comprobantes.ToListAsync());
    }

    public async Task<IActionResult> Details(long? idcomprobante)
    {
        if (idcomprobante == null)
        {
            return NotFound();
        }

        var comprobantepago = await _context.ComprobantesPago
            .Include(c => c.Pedido)
            .ThenInclude(p => p.Cliente)
            .FirstOrDefaultAsync(m => m.IdComprobante == idcomprobante);

        if (comprobantepago == null)
        {
            return NotFound();
        }

        return View(comprobantepago);
    }

    public IActionResult Create()
    {
        ViewBag.Pedidos = new SelectList(
            _context.Pedidos.Include(p => p.Cliente),
            "IdPedido",
            "IdPedido"
        );

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("IdPedido,TipoComprobante,Serie,Correlativo,MetodoPago")] ComprobantePago comprobantepago)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .FirstOrDefaultAsync(p => p.IdPedido == comprobantepago.IdPedido);

        if (pedido == null)
        {
            return NotFound();
        }

        comprobantepago.FechaEmision = DateTime.Now;
        comprobantepago.MontoTotal = pedido.Total;
        comprobantepago.MontoSubtotal = Math.Round(pedido.Total / 1.18m, 2);
        comprobantepago.MontoIgv = Math.Round(pedido.Total - comprobantepago.MontoSubtotal, 2);

        comprobantepago.ClienteDocumento = pedido.Cliente?.DniRuc ?? "00000000";
        comprobantepago.ClienteNombreORazonSocial =
            $"{pedido.Cliente?.Nombre} {pedido.Cliente?.Apellido}".Trim();

        if (ModelState.IsValid)
        {
            _context.Add(comprobantepago);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Pedidos = new SelectList(_context.Pedidos, "IdPedido", "IdPedido", comprobantepago.IdPedido);
        return View(comprobantepago);
    }

    public async Task<IActionResult> Edit(long? idcomprobante)
    {
        if (idcomprobante == null)
        {
            return NotFound();
        }

        var comprobantepago = await _context.ComprobantesPago.FindAsync(idcomprobante);

        if (comprobantepago == null)
        {
            return NotFound();
        }

        ViewBag.Pedidos = new SelectList(_context.Pedidos, "IdPedido", "IdPedido", comprobantepago.IdPedido);

        return View(comprobantepago);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        long? idcomprobante,
        [Bind("IdComprobante,IdPedido,TipoComprobante,Serie,Correlativo,MetodoPago")] ComprobantePago comprobantepago)
    {
        if (idcomprobante != comprobantepago.IdComprobante)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .FirstOrDefaultAsync(p => p.IdPedido == comprobantepago.IdPedido);

        if (pedido == null)
        {
            return NotFound();
        }

        comprobantepago.FechaEmision = DateTime.Now;
        comprobantepago.MontoTotal = pedido.Total;
        comprobantepago.MontoSubtotal = Math.Round(pedido.Total / 1.18m, 2);
        comprobantepago.MontoIgv = Math.Round(pedido.Total - comprobantepago.MontoSubtotal, 2);
        comprobantepago.ClienteDocumento = pedido.Cliente?.DniRuc ?? "00000000";
        comprobantepago.ClienteNombreORazonSocial =
            $"{pedido.Cliente?.Nombre} {pedido.Cliente?.Apellido}".Trim();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(comprobantepago);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComprobantePagoExists(comprobantepago.IdComprobante))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Pedidos = new SelectList(_context.Pedidos, "IdPedido", "IdPedido", comprobantepago.IdPedido);
        return View(comprobantepago);
    }

    public async Task<IActionResult> Delete(long? idcomprobante)
    {
        if (idcomprobante == null)
        {
            return NotFound();
        }

        var comprobantepago = await _context.ComprobantesPago
            .Include(c => c.Pedido)
            .FirstOrDefaultAsync(m => m.IdComprobante == idcomprobante);

        if (comprobantepago == null)
        {
            return NotFound();
        }

        return View(comprobantepago);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long? idcomprobante)
    {
        var comprobantepago = await _context.ComprobantesPago.FindAsync(idcomprobante);

        if (comprobantepago != null)
        {
            _context.ComprobantesPago.Remove(comprobantepago);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ComprobantePagoExists(long? idcomprobante)
    {
        return _context.ComprobantesPago.Any(e => e.IdComprobante == idcomprobante);
    }
}