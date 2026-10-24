using GestionHotelera.Data;
using GestionHotelera.Models;
using GestionHotelera.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Controllers;

public class HuespedesController : Controller
{
    private readonly AppDbContext _context;

    public HuespedesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string? busqueda,
        int pagina = 1)
    {
        busqueda = busqueda?.Trim();

        ViewData["Busqueda"] = busqueda;

        IQueryable<Huesped> consulta = _context.Huespedes
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            consulta = consulta.Where(h =>
                h.NombreHuesped.Contains(busqueda)
                || h.Celular.Contains(busqueda));
        }

        var consultaOrdenada = consulta
            .OrderBy(h => h.NombreHuesped)
            .ThenBy(h => h.IdHuesped);

        var datos = await ListaPaginada<Huesped>.CrearAsync(
            consultaOrdenada,
            pagina,
            10);

        return View(datos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var huesped = await _context.Huespedes
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.IdHuesped == id);

        if (huesped is null)
        {
            return NotFound();
        }

        return View(huesped);
    }

    public IActionResult Create()
    {
        return View(new HuespedFormularioViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        HuespedFormularioViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var huesped = new Huesped
        {
            NombreHuesped = modelo.NombreHuesped.Trim(),
            Celular = modelo.Celular.Trim()
        };

        _context.Huespedes.Add(huesped);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo registrar el huésped. Intente nuevamente.");

            return View(modelo);
        }

        TempData["Exito"] = "Huésped registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var huesped = await _context.Huespedes
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.IdHuesped == id);

        if (huesped is null)
        {
            return NotFound();
        }

        var modelo = new HuespedFormularioViewModel
        {
            IdHuesped = huesped.IdHuesped,
            NombreHuesped = huesped.NombreHuesped,
            Celular = huesped.Celular
        };

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        HuespedFormularioViewModel modelo)
    {
        if (id != modelo.IdHuesped)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var huesped = await _context.Huespedes
            .FirstOrDefaultAsync(h => h.IdHuesped == id);

        if (huesped is null)
        {
            return NotFound();
        }

        huesped.NombreHuesped = modelo.NombreHuesped.Trim();
        huesped.Celular = modelo.Celular.Trim();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(
                string.Empty,
                "El registro cambió o fue eliminado. Vuelva al listado.");

            return View(modelo);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudieron guardar los cambios. Intente nuevamente.");

            return View(modelo);
        }

        TempData["Exito"] = "Huésped actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var huesped = await _context.Huespedes
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.IdHuesped == id);

        if (huesped is null)
        {
            return NotFound();
        }

        bool tieneReservas = await _context.Reservas
            .AnyAsync(r => r.IdHuesped == id);

        if (tieneReservas)
        {
            TempData["Error"] =
                "No se puede eliminar el huésped porque tiene reservas.";

            return RedirectToAction(nameof(Index));
        }

        return View(huesped);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var huesped = await _context.Huespedes
            .FirstOrDefaultAsync(h => h.IdHuesped == id);

        if (huesped is null)
        {
            return NotFound();
        }

        bool tieneReservas = await _context.Reservas
            .AnyAsync(r => r.IdHuesped == id);

        if (tieneReservas)
        {
            TempData["Error"] =
                "No se puede eliminar el huésped porque tiene reservas.";

            return RedirectToAction(nameof(Index));
        }

        _context.Huespedes.Remove(huesped);

        try
        {
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Huésped eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] =
                "No se pudo eliminar el huésped. Puede tener registros relacionados.";
        }

        return RedirectToAction(nameof(Index));
    }
}