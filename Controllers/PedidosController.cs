using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Data;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Mesa)
                .Include(p => p.DetallePedidos!)
                    .ThenInclude(d => d.Platillo)
                .FirstOrDefaultAsync(m => m.IdPedido == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: PEDIDOS/Create
        public async Task<IActionResult> Create()
        {
            // 1. Lógica inteligente según el rol
            if (User.IsInRole("Cliente"))
            {
                var claimId = User.FindFirst("UsuarioId")?.Value;
                long idCliente = string.IsNullOrEmpty(claimId) ? 0 : long.Parse(claimId);
                var clienteInfo = await _context.Clientes.FindAsync(idCliente);

                ViewBag.EsClienteWeb = true;
                ViewBag.IdClienteLogueado = idCliente;
                ViewBag.NombreClienteLogueado = clienteInfo != null ? $"{clienteInfo.Nombre} {clienteInfo.Apellido}".Trim() : "Cliente Web";
            }
            else
            {
                ViewBag.EsClienteWeb = false;
                ViewBag.Clientes = new SelectList(_context.Clientes, "IdCliente", "Nombre");
            }

            // Siempre cargamos mesas y empleados (aunque se oculten para el cliente web)
            ViewBag.Mesas = new SelectList(_context.Mesas, "IdMesa", "NumeroMesa");
            ViewBag.Empleados = new SelectList(_context.Empleados, "IdEmpleado", "Nombre");

            // Traemos los platillos disponibles para el selector dinámico
            ViewBag.Platillos = _context.Platillos
                .Where(p => p.Disponibilidad == true)
                .Select(p => new {
                    IdPlatillo = p.IdPlatillo,
                    NombrePrecio = $"{p.NombrePlatillo} (S/ {p.PrecioVenta})"
                }).ToList();

            return View();
        }

        // POST: PEDIDOS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,IdEmpleado,IdMesa,TipoPedido,DetallePedidos")] Pedido pedido)
        {
            // 1. BLINDAJE INICIAL
            if (User.IsInRole("Cliente"))
            {
                var claimId = User.FindFirst("UsuarioId")?.Value;
                pedido.IdCliente = string.IsNullOrEmpty(claimId) ? 0 : long.Parse(claimId);
                pedido.TipoPedido = "Web - Recojo en local";
                pedido.IdEmpleado = null;
                pedido.IdMesa = null;
            }

            // 2. VALIDACIÓN
            if (pedido.DetallePedidos == null || !pedido.DetallePedidos.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un platillo al pedido.");
            }

            if (ModelState.IsValid)
            {
                decimal totalConsolidado = 0;

                foreach (var detalle in pedido.DetallePedidos!)
                {
                    var platillo = await _context.Platillos.FindAsync(detalle.IdPlatillo);
                    if (platillo != null)
                    {
                        detalle.PrecioUnitario = platillo.PrecioVenta;
                        totalConsolidado += (detalle.Cantidad * platillo.PrecioVenta);

                        var ingredientesReceta = await _context.RecetasPlatillo
                            .Where(r => r.IdPlatillo == detalle.IdPlatillo)
                            .ToListAsync();

                        foreach (var ingrediente in ingredientesReceta)
                        {
                            var insumoAlmacen = await _context.Insumos.FindAsync(ingrediente.IdInsumo);
                            if (insumoAlmacen != null)
                            {
                                int cantidadADescontar = ingrediente.CantidadRequerida * detalle.Cantidad;
                                insumoAlmacen.StockActual -= cantidadADescontar;
                                _context.Update(insumoAlmacen);
                            }
                        }
                    }
                }

                pedido.Total = totalConsolidado;
                pedido.FechaPedido = DateTime.Now;
                pedido.EstadoPedido = "En Espera";

                _context.Add(pedido);
                await _context.SaveChangesAsync();

                if (User.IsInRole("Cliente")) return RedirectToAction("Index", "ClientePortal");
                return RedirectToAction(nameof(Index));
            }

            // ========================================================================
            // 🔥 CORRECCIÓN: RECARGAR LA SEGURIDAD SI EL FORMULARIO FALLA
            // ========================================================================
            if (User.IsInRole("Cliente"))
            {
                var claimId = User.FindFirst("UsuarioId")?.Value;
                long idCliente = string.IsNullOrEmpty(claimId) ? 0 : long.Parse(claimId);
                var clienteInfo = await _context.Clientes.FindAsync(idCliente);

                ViewBag.EsClienteWeb = true;
                ViewBag.IdClienteLogueado = idCliente;
                ViewBag.NombreClienteLogueado = clienteInfo != null ? $"{clienteInfo.Nombre} {clienteInfo.Apellido}".Trim() : "Cliente Web";
            }
            else
            {
                ViewBag.EsClienteWeb = false;
                ViewBag.Clientes = new SelectList(_context.Clientes, "IdCliente", "Nombre", pedido.IdCliente);
            }

            ViewBag.Mesas = new SelectList(_context.Mesas, "IdMesa", "NumeroMesa", pedido.IdMesa);
            ViewBag.Empleados = new SelectList(_context.Empleados, "IdEmpleado", "Nombre", pedido.IdEmpleado);
            ViewBag.Platillos = _context.Platillos.Where(p => p.Disponibilidad == true)
                .Select(p => new {
                    IdPlatillo = p.IdPlatillo,
                    NombrePrecio = $"{p.NombrePlatillo} (S/ {p.PrecioVenta})"
                }).ToList();

            return View(pedido);
        }

        // POST: PEDIDOS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("IdPedido,IdCliente,IdEmpleado,IdMesa,TipoPedido,EstadoPedido,DetallePedidos")] Pedido pedido)
        {
            if (id != pedido.IdPedido)
            {
                return NotFound();
            }

            if (pedido.DetallePedidos == null || !pedido.DetallePedidos.Any())
            {
                ModelState.AddModelError("", "No puede dejar un pedido sin ningún platillo.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Eliminar los detalles anteriores para evitar duplicados en llaves compuestas
                    var detallesAntiguos = _context.DetallePedidos.Where(d => d.IdPedido == pedido.IdPedido);
                    _context.DetallePedidos.RemoveRange(detallesAntiguos);
                    await _context.SaveChangesAsync();

                    // 2. Calcular los nuevos subtotales y actualizar precios desde el servidor
                    decimal totalConsolidado = 0;
                    foreach (var detalle in pedido.DetallePedidos!)
                    {
                        detalle.IdPedido = pedido.IdPedido;
                        var platillo = await _context.Platillos.FindAsync(detalle.IdPlatillo);
                        if (platillo != null)
                        {
                            detalle.PrecioUnitario = platillo.PrecioVenta;
                            totalConsolidado += (detalle.Cantidad * platillo.PrecioVenta);
                        }
                        _context.Add(detalle);
                    }

                    // 3. Actualizar la cabecera
                    pedido.Total = totalConsolidado;
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.IdPedido)) return NotFound();
                    else throw;
                }
            }

            // Si algo falla, se recargan las listas
            ViewBag.Clientes = new SelectList(_context.Clientes, "IdCliente", "Nombre", pedido.IdCliente);
            ViewBag.Mesas = new SelectList(_context.Mesas, "IdMesa", "NumeroMesa", pedido.IdMesa);
            ViewBag.Empleados = new SelectList(_context.Empleados, "IdEmpleado", "Nombre", pedido.IdEmpleado);
            ViewBag.Platillos = _context.Platillos.Where(p => p.Disponibilidad == true)
                .Select(p => new { IdPlatillo = p.IdPlatillo, NombrePrecio = $"{p.NombrePlatillo} (S/ {p.PrecioVenta})" }).ToList();

            return View(pedido);
        }

        // GET: PEDIDOS/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Empleado)
                .Include(p => p.Mesa)
                .FirstOrDefaultAsync(m => m.IdPedido == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: PEDIDOS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            // Validación de seguridad: si es nulo, no hacemos nada
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(id);

            // Verificamos que realmente se haya encontrado el objeto
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(long? id)
        {
            return _context.Pedidos.Any(e => e.IdPedido == id);
        }
    }
}