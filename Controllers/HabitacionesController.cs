using GestionHotelera.Data;
using GestionHotelera.Models;
using GestionHotelera.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Controllers;

public class HabitacionesController : Controller
{
    private readonly AppDbContext _context;
    public HabitacionesController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? busqueda, string? tipo, int pagina = 1)
    {
        busqueda = busqueda?.Trim();
        ViewData["Busqueda"] = busqueda;
        IQueryable<Habitacion> consulta = _context.Habitaciones.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            if (int.TryParse(busqueda, out var numero)) consulta = consulta.Where(x => x.Numero == numero);
            else consulta = consulta.Where(x => false);
        }
        if (!string.IsNullOrWhiteSpace(tipo)) consulta = consulta.Where(x => x.Tipo == tipo);
        ViewData["Tipo"] = tipo;
        var datos = await ListaPaginada<Habitacion>.CrearAsync(
            consulta.OrderBy(x => x.Numero).ThenBy(x => x.IdHabitacion), pagina, 10);
        return View(datos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var dato = await _context.Habitaciones.AsNoTracking().FirstOrDefaultAsync(x => x.IdHabitacion == id);
        return dato == null ? NotFound() : View(dato);
    }

    public IActionResult Create() => View(new HabitacionFormularioViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HabitacionFormularioViewModel modelo)
    {
        if (await _context.Habitaciones.AnyAsync(x => x.Numero == modelo.Numero))
            ModelState.AddModelError(nameof(modelo.Numero), "Ya existe una habitación con ese número.");
        if (!ModelState.IsValid) return View(modelo);
        var dato = new Habitacion {
            Numero = modelo.Numero,
            Tipo = modelo.Tipo.Trim(),
            Precio = modelo.Precio
        };
        _context.Habitaciones.Add(dato);
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException) {
            ModelState.AddModelError("", "No se pudo guardar. Compruebe los datos e intente nuevamente.");
            return View(modelo);
        }
        TempData["Exito"] = "Habitacion registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var dato = await _context.Habitaciones.AsNoTracking().FirstOrDefaultAsync(x => x.IdHabitacion == id);
        if (dato == null) return NotFound();
        return View(new HabitacionFormularioViewModel {
            IdHabitacion = dato.IdHabitacion,
            Numero = dato.Numero,
            Tipo = dato.Tipo,
            Precio = dato.Precio
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HabitacionFormularioViewModel modelo)
    {
        if (id != modelo.IdHabitacion) return NotFound();
        if (await _context.Habitaciones.AnyAsync(x => x.Numero == modelo.Numero && x.IdHabitacion != id))
            ModelState.AddModelError(nameof(modelo.Numero), "Ya existe una habitación con ese número.");
        if (!ModelState.IsValid) return View(modelo);
        var dato = await _context.Habitaciones.FirstOrDefaultAsync(x => x.IdHabitacion == id);
        if (dato == null) return NotFound();
        dato.Numero = modelo.Numero;
        dato.Tipo = modelo.Tipo.Trim();
        dato.Precio = modelo.Precio;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) {
            ModelState.AddModelError("", "El registro fue modificado o eliminado. Vuelva al listado.");
            return View(modelo);
        }
        catch (DbUpdateException) {
            ModelState.AddModelError("", "No se pudieron guardar los cambios.");
            return View(modelo);
        }
        TempData["Exito"] = "Habitacion actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var dato = await _context.Habitaciones.AsNoTracking().FirstOrDefaultAsync(x => x.IdHabitacion == id);
        return dato == null ? NotFound() : View(dato);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var dato = await _context.Habitaciones.FirstOrDefaultAsync(x => x.IdHabitacion == id);
        if (dato == null) return NotFound();
        if (await _context.Reservas.AnyAsync(x => x.IdHabitacion == id))
        {
            TempData["Error"] = "No se puede eliminar porque tiene reservas asociadas.";
            return RedirectToAction(nameof(Index));
        }
        _context.Habitaciones.Remove(dato);
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException) {
            TempData["Error"] = "No se puede eliminar porque existen registros relacionados.";
            return RedirectToAction(nameof(Index));
        }
        TempData["Exito"] = "Habitacion eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
