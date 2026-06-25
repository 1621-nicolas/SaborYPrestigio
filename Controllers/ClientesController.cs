
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;
using SaborPrestigioMVC.Data;

public class ClientesController : Controller
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: CLIENTES
    public async Task<IActionResult> Index(string buscar, int pagina = 1)
    {
        int registrosPorPagina = 10;

        var clientes = _context.Clientes.AsQueryable();

        if (!string.IsNullOrEmpty(buscar))
        {
            clientes = clientes.Where(c =>
                c.Nombre.Contains(buscar) ||
                c.Apellido.Contains(buscar) ||
                c.DniRuc.Contains(buscar) ||
                c.Email.Contains(buscar));
        }

        int totalRegistros = await clientes.CountAsync();

        var listaClientes = await clientes
            .OrderBy(c => c.IdCliente)
            .Skip((pagina - 1) * registrosPorPagina)
            .Take(registrosPorPagina)
            .ToListAsync();

        ViewBag.Buscar = buscar;
        ViewBag.PaginaActual = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

        return View(listaClientes);
    }

    // GET: CLIENTES/Details/5
    public async Task<IActionResult> Details(long? idcliente)
    {
        if (idcliente == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(m => m.IdCliente == idcliente);
        if (cliente == null)
        {
            return NotFound();
        }

        return View(cliente);
    }






    // GET: CLIENTES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CLIENTES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.

    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
      [Bind("DniRuc,Nombre,Apellido,Telefono,Email")] Cliente cliente)
    {
        var dniExiste = await _context.Clientes
            .AnyAsync(c => c.DniRuc == cliente.DniRuc);

        if (dniExiste)
        {
            ModelState.AddModelError(
                "DniRuc",
                "Este DNI ya fue registrado.");
        }

        if (ModelState.IsValid)
        {
            cliente.FechaRegistro = DateTime.Now;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(cliente);
    }

    // GET: CLIENTES/Edit/5
    public async Task<IActionResult> Edit(long? idcliente)
    {
        if (idcliente == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes.FindAsync(idcliente);
        if (cliente == null)
        {
            return NotFound();
        }
        return View(cliente);
    }

    // POST: CLIENTES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long? idcliente, [Bind("IdCliente,DniRuc,Nombre,Apellido,Telefono,Email")] Cliente cliente)
    {
        if (idcliente != cliente.IdCliente)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var dniExiste = await _context.Clientes.AnyAsync(c =>
    c.DniRuc == cliente.DniRuc &&
    c.IdCliente != cliente.IdCliente);

            if (dniExiste)
            {
                ModelState.AddModelError(
                    "DniRuc",
                    "Este DNI ya pertenece a otro cliente.");
            }
            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(cliente.IdCliente))
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
        return View(cliente);
    }

    // GET: CLIENTES/Delete/5
    public async Task<IActionResult> Delete(long? idcliente)
    {
        if (idcliente == null)
        {
            return NotFound();
        }

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(m => m.IdCliente == idcliente);
        if (cliente == null)
        {
            return NotFound();
        }

        return View(cliente);
    }

    // POST: CLIENTES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long? idcliente)
    {
        var cliente = await _context.Clientes.FindAsync(idcliente);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ClienteExists(long? idcliente)
    {
        return _context.Clientes.Any(e => e.IdCliente == idcliente);
    }
}
