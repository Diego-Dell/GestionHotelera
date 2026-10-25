
using GestionHotelera.Data;
using GestionHotelera.Models;
using GestionHotelera.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Controllers;

public class ServiciosController : Controller
{
    private readonly AppDbContext _context;

    public ServiciosController(AppDbContext context)
    {
        _context = context;
    }

    // LISTAR, BUSCAR Y PAGINAR
    public async Task<IActionResult> Index(
        string? busqueda, int pagina = 1)
    {
        busqueda = busqueda?.Trim();
        ViewData["Busqueda"] = busqueda;

        IQueryable<Servicio> consulta =
            _context.Servicios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            consulta = consulta.Where(s =>
                s.Nombre.Contains(busqueda));
        }

        var datos = await ListaPaginada<Servicio>.CrearAsync(
            consulta.OrderBy(s => s.Nombre)
                    .ThenBy(s => s.IdServicio),
            pagina,
            10
        );

        return View(datos);
    }

    // VER DETALLES
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var servicio = await _context.Servicios
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdServicio == id);

        if (servicio == null)
            return NotFound();

        return View(servicio);
    }

    // MOSTRAR FORMULARIO PARA REGISTRAR
    public IActionResult Create()
    {
        return View(new ServicioFormularioViewModel());
    }

    // GUARDAR SERVICIO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ServicioFormularioViewModel modelo)
    {
        string nombre = (modelo.Nombre ?? "").Trim();

        // Validar el nombre
        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(
                nameof(modelo.Nombre),
                "Debe ingresar el nombre del servicio.");
        }
        else
        {
            // Evitar nombres duplicados
            string nombreComparacion = nombre.ToUpper();

            bool existe = await _context.Servicios
                .AnyAsync(s =>
                    s.Nombre.Trim().ToUpper()
                    == nombreComparacion);

            if (existe)
            {
                ModelState.AddModelError(
                    nameof(modelo.Nombre),
                    "Ya existe un servicio con este nombre.");
            }
        }

        if (!ModelState.IsValid)
            return View(modelo);

        var servicio = new Servicio
        {
            Nombre = nombre,
            Precio = modelo.Precio
        };

        _context.Servicios.Add(servicio);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "",
                "No se pudo guardar el servicio.");
            return View(modelo);
        }

        TempData["Exito"] =
            "Servicio registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR FORMULARIO PARA EDITAR
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var servicio = await _context.Servicios
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdServicio == id);

        if (servicio == null)
            return NotFound();

        var modelo = new ServicioFormularioViewModel
        {
            IdServicio = servicio.IdServicio,
            Nombre = servicio.Nombre,
            Precio = servicio.Precio
        };

        return View(modelo);
    }

    // GUARDAR CAMBIOS
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, ServicioFormularioViewModel modelo)
    {
        if (id != modelo.IdServicio)
            return NotFound();

        string nombre = (modelo.Nombre ?? "").Trim();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(
                nameof(modelo.Nombre),
                "Debe ingresar el nombre del servicio.");
        }
        else
        {
            string nombreComparacion = nombre.ToUpper();

            // Comprobar duplicados, excluyendo
            // el servicio que estamos editando
            bool existe = await _context.Servicios
                .AnyAsync(s =>
                    s.IdServicio != id &&
                    s.Nombre.Trim().ToUpper()
                    == nombreComparacion);

            if (existe)
            {
                ModelState.AddModelError(
                    nameof(modelo.Nombre),
                    "Ya existe otro servicio con este nombre.");
            }
        }

        if (!ModelState.IsValid)
            return View(modelo);

        var servicio = await _context.Servicios
            .FirstOrDefaultAsync(s => s.IdServicio == id);

        if (servicio == null)
            return NotFound();

        servicio.Nombre = nombre;
        servicio.Precio = modelo.Precio;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(
                "",
                "El servicio fue modificado o eliminado.");
            return View(modelo);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                "",
                "No se pudieron guardar los cambios.");
            return View(modelo);
        }

        TempData["Exito"] =
            "Servicio actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR CONFIRMACION DE ELIMINACION
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var servicio = await _context.Servicios
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.IdServicio == id);

        if (servicio == null)
            return NotFound();

        return View(servicio);
    }

    // ELIMINAR SERVICIO
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var servicio = await _context.Servicios
            .FirstOrDefaultAsync(s => s.IdServicio == id);

        if (servicio == null)
            return NotFound();

        // No eliminar servicios asociados a reservas
        bool tieneReservas = await _context.ReservaServicios
            .AnyAsync(rs => rs.IdServicio == id);

        if (tieneReservas)
        {
            TempData["Error"] =
                "No se puede eliminar el servicio " +
                "porque tiene reservas asociadas.";

            return RedirectToAction(nameof(Index));
        }

        _context.Servicios.Remove(servicio);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["Error"] =
                "No se puede eliminar porque existen " +
                "registros relacionados.";

            return RedirectToAction(nameof(Index));
        }

        TempData["Exito"] =
            "Servicio eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
