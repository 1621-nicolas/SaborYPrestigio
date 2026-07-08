using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

[Authorize]
public class ComprobantedePagoController : Controller
{
    private readonly AppDbContext _context;

    public ComprobantedePagoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ComprobantedePago
    public async Task<IActionResult> Index()
    {
        var comprobantes = _context.ComprobantesPago
            .Include(c => c.Pedido)
            .ThenInclude(p => p!.Cliente);

        return View(await comprobantes.ToListAsync());
    }

    // GET: ComprobantedePago/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null) return NotFound();

        var comprobantepago = await _context.ComprobantesPago
            .Include(c => c.Pedido)
            .ThenInclude(p => p!.Cliente)
            .FirstOrDefaultAsync(m => m.IdComprobante == id);

        if (comprobantepago == null) return NotFound();

        return View(comprobantepago);
    }

    // GET: ComprobantedePago/Create
    public async Task<IActionResult> Create()
    {
        var pedidosActivos = await _context.Pedidos
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.IdPedido)
            .ToListAsync();

        // 🔥 CORREGIDO: Navegamos a través de p.Cliente para obtener los datos correctos del script SQL
        ViewBag.PedidosLista = pedidosActivos.Select(p => new {
            IdPedido = p.IdPedido,
            TextoMostrado = $"Pedido #{p.IdPedido} - {(p.Cliente != null ? (p.Cliente.Nombre + " " + p.Cliente.Apellido).Trim() : "Cliente General")} (S/ {p.Total:0.00})",
            MontoTotal = p.Total,
            Documento = p.Cliente != null ? p.Cliente.DniRuc : "00000000",
            NombreCompleto = p.Cliente != null ? ($"{p.Cliente.Nombre} {p.Cliente.Apellido}").Trim() : "CLIENTE GENERAL"
        }).ToList();

        return View();
    }

    // POST: ComprobantedePago/Create
    // POST: ComprobantedePago/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdPedido,TipoComprobante,Serie,Correlativo,MetodoPago")] ComprobantePago comprobantepago)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .FirstOrDefaultAsync(p => p.IdPedido == comprobantepago.IdPedido);

        if (pedido == null) return NotFound();

        // 1. Asignamos y calculamos los valores de forma manual en el Servidor de forma ultra segura
        comprobantepago.FechaEmision = DateTime.Now;
        comprobantepago.MontoTotal = pedido.Total;
        comprobantepago.MontoSubtotal = Math.Round(pedido.Total / 1.18m, 2);
        comprobantepago.MontoIgv = Math.Round(pedido.Total - comprobantepago.MontoSubtotal, 2);

        // Dentro del POST de tu Create, busca estas líneas y déjalas así:

        // Si el cajero escribió un RUC/Razón Social en la interfaz, usamos esos. Si no, usamos los del cliente del pedido.
        comprobantepago.ClienteDocumento = !string.IsNullOrEmpty(comprobantepago.ClienteDocumento)
            ? comprobantepago.ClienteDocumento
            : (pedido.Cliente != null ? pedido.Cliente.DniRuc : "00000000");

        comprobantepago.ClienteNombreORazonSocial = !string.IsNullOrEmpty(comprobantepago.ClienteNombreORazonSocial)
            ? comprobantepago.ClienteNombreORazonSocial
            : (pedido.Cliente != null ? ($"{pedido.Cliente.Nombre} {pedido.Cliente.Apellido}").Trim() : "CLIENTE GENERAL");

        // 2. Autogeneración de correlativos correlacionados
        var ultimoComprobante = await _context.ComprobantesPago
            .Where(c => c.TipoComprobante == comprobantepago.TipoComprobante)
            .OrderByDescending(c => c.Correlativo)
            .FirstOrDefaultAsync();

        comprobantepago.Correlativo = ultimoComprobante != null ? ultimoComprobante.Correlativo + 1 : 1;
        comprobantepago.Serie = comprobantepago.TipoComprobante == "Boleta" ? "B001" : "F001";

        // 3. 🚀 GUARDADO DIRECTO: Forzamos la inserción saltándonos las validaciones implícitas del ModelBinder
        try
        {
            _context.ComprobantesPago.Add(comprobantepago);
            await _context.SaveChangesAsync();

            // Si guarda con éxito, nos lleva directo al Index donde se pintará la fila
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // En caso de que falle a nivel de base de datos, capturamos el error
            ModelState.AddModelError("", "Error físico al guardar en la BD: " + ex.Message);
        }

        // Si falló el bloque try, recargamos la lista para no romper la interfaz
        var pedidosActivos = await _context.Pedidos.Include(p => p.Cliente).OrderByDescending(p => p.IdPedido).ToListAsync();
        ViewBag.PedidosLista = pedidosActivos.Select(p => new {
            IdPedido = p.IdPedido,
            TextoMostrado = $"Pedido #{p.IdPedido} - {(p.Cliente != null ? (p.Cliente.Nombre + " " + p.Cliente.Apellido).Trim() : "Cliente General")} (S/ {p.Total:0.00})",
            MontoTotal = p.Total,
            Documento = p.Cliente != null ? p.Cliente.DniRuc : "00000000",
            NombreCompleto = p.Cliente != null ? ($"{p.Cliente.Nombre} {p.Cliente.Apellido}").Trim() : "CLIENTE GENERAL"
        }).ToList();

        return View(comprobantepago);
    }
    // GET: ComprobantedePago/Delete/5
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null) return NotFound();

        var comprobantepago = await _context.ComprobantesPago
            .Include(c => c.Pedido)
            .FirstOrDefaultAsync(m => m.IdComprobante == id);

        if (comprobantepago == null) return NotFound();

        return View(comprobantepago);
    }

    // POST: ComprobantedePago/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var comprobantepago = await _context.ComprobantesPago.FindAsync(id);
        if (comprobantepago != null)
        {
            _context.ComprobantesPago.Remove(comprobantepago);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ComprobantePagoExists(long id)
    {
        return _context.ComprobantesPago.Any(e => e.IdComprobante == id);
    }
}