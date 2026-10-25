
using GestionHotelera.Data;
using GestionHotelera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using GestionHotelera.ViewModels;


namespace GestionHotelera.Controllers;

public class ReservasController : Controller
{
    private readonly AppDbContext _context;

    public ReservasController(AppDbContext context)
    {
        _context = context;
    }

    // LISTAR RESERVAS CON FILTROS Y PAGINACIÓN
    public async Task<IActionResult> Index(
        int? idHuesped,
        int? idHabitacion,
        string? estado,
        DateTime? desde,
        DateTime? hasta,
        int pagina = 1)
    {
        IQueryable<Reserva> consulta = _context.Reservas
            .AsNoTracking()
            .Include(r => r.Huesped)
            .Include(r => r.Habitacion);

        // Filtrar por huésped
        if (idHuesped.HasValue)
        {
            consulta = consulta.Where(r =>
                r.IdHuesped == idHuesped.Value);
        }

        // Filtrar por habitación
        if (idHabitacion.HasValue)
        {
            consulta = consulta.Where(r =>
                r.IdHabitacion == idHabitacion.Value);
        }

        // Filtrar por estado
        if (!string.IsNullOrWhiteSpace(estado))
        {
            consulta = consulta.Where(r =>
                r.Estado == estado);
        }

        // Filtrar por fecha de ingreso
        if (desde.HasValue)
        {
            consulta = consulta.Where(r =>
                r.FechaInicio >= desde.Value.Date);
        }

        // Filtrar por fecha de salida
        if (hasta.HasValue)
        {
            DateTime limite = hasta.Value.Date.AddDays(1);

            consulta = consulta.Where(r =>
                r.FechaFin < limite);
        }

        // Mantener los filtros seleccionados
        ViewData["IdHuesped"] = idHuesped;
        ViewData["IdHabitacion"] = idHabitacion;
        ViewData["Estado"] = estado;
        ViewData["Desde"] = desde;
        ViewData["Hasta"] = hasta;

        await CargarListasAsync(idHuesped, idHabitacion);

        // Ordenar y paginar
        var reservas = await ListaPaginada<Reserva>.CrearAsync(
            consulta
                .OrderByDescending(r => r.FechaInicio)
                .ThenByDescending(r => r.IdReserva),
            pagina,
            10
        );

        return View(reservas);
    }

    // CARGAR HUÉSPEDES Y HABITACIONES
    private async Task CargarListasAsync(
        int? idHuesped = null,
        int? idHabitacion = null)
    {
        var huespedes = await _context.Huespedes
            .AsNoTracking()
            .OrderBy(h => h.NombreHuesped)
            .ToListAsync();

        var habitaciones = await _context.Habitaciones
            .AsNoTracking()
            .OrderBy(h => h.Numero)
            .ToListAsync();

        ViewBag.Huespedes = new SelectList(
            huespedes,
            "IdHuesped",
            "NombreHuesped",
            idHuesped
        );

        var listaHabitaciones = habitaciones.Select(h => new
        {
            h.IdHabitacion,
            Descripcion = $"{h.Numero} - {h.Tipo}"
        }).ToList();

        ViewBag.Habitaciones = new SelectList(
            listaHabitaciones,
            "IdHabitacion",
            "Descripcion",
            idHabitacion
        );
    }

    // MOSTRAR FORMULARIO DE REGISTRO
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();

        return View(new ReservaFormularioViewModel());
    }

    // GUARDAR NUEVA RESERVA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ReservaFormularioViewModel modelo)
    {
        // Validar fechas
        if (modelo.FechaFin.Date <= modelo.FechaInicio.Date)
        {
            ModelState.AddModelError(
                nameof(modelo.FechaFin),
                "La fecha de salida debe ser posterior al ingreso.");
        }

        // Validar que exista el huésped
        bool existeHuesped = await _context.Huespedes
            .AnyAsync(h => h.IdHuesped == modelo.IdHuesped);

        if (!existeHuesped)
        {
            ModelState.AddModelError(
                nameof(modelo.IdHuesped),
                "Seleccione un huésped válido.");
        }

        // Validar que exista la habitación
        bool existeHabitacion = await _context.Habitaciones
            .AnyAsync(h => h.IdHabitacion == modelo.IdHabitacion);

        if (!existeHabitacion)
        {
            ModelState.AddModelError(
                nameof(modelo.IdHabitacion),
                "Seleccione una habitación válida.");
        }

        // Validar estado
        if (modelo.Estado != "Pendiente" &&
            modelo.Estado != "Confirmada" &&
            modelo.Estado != "Cancelada")
        {
            ModelState.AddModelError(
                nameof(modelo.Estado),
                "Seleccione un estado válido.");
        }

        // Verificar disponibilidad
        if (modelo.FechaFin.Date > modelo.FechaInicio.Date &&
            existeHabitacion &&
            modelo.Estado != "Cancelada")
        {
            bool ocupada = await _context.Reservas
                .AnyAsync(r =>
                    r.IdHabitacion == modelo.IdHabitacion &&
                    r.Estado != "Cancelada" &&
                    r.FechaInicio < modelo.FechaFin.Date &&
                    modelo.FechaInicio.Date < r.FechaFin);

            if (ocupada)
            {
                ModelState.AddModelError(
                    nameof(modelo.IdHabitacion),
                    "La habitación ya tiene una reserva en esas fechas.");
            }
        }

        // Si hay errores, mostrar nuevamente el formulario
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(
                modelo.IdHuesped,
                modelo.IdHabitacion);

            return View(modelo);
        }

        // Crear la reserva
        var reserva = new Reserva
        {
            IdHuesped = modelo.IdHuesped,
            IdHabitacion = modelo.IdHabitacion,
            FechaInicio = modelo.FechaInicio.Date,
            FechaFin = modelo.FechaFin.Date,
            MetodoPago = modelo.MetodoPago.Trim(),
            Estado = modelo.Estado
        };

        // Guardar en la base de datos
        _context.Reservas.Add(reserva);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo guardar la reserva.");

            await CargarListasAsync(
                modelo.IdHuesped,
                modelo.IdHabitacion);

            return View(modelo);
        }

        TempData["Exito"] = "Reserva registrada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR FORMULARIO DE EDICIÓN
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reserva = await _context.Reservas
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva == null)
        {
            return NotFound();
        }

        var modelo = new ReservaFormularioViewModel
        {
            IdReserva = reserva.IdReserva,
            IdHuesped = reserva.IdHuesped,
            IdHabitacion = reserva.IdHabitacion,
            FechaInicio = reserva.FechaInicio,
            FechaFin = reserva.FechaFin,
            MetodoPago = reserva.MetodoPago,
            Estado = reserva.Estado
        };

        await CargarListasAsync(
            modelo.IdHuesped,
            modelo.IdHabitacion);

        return View(modelo);
    }

    // GUARDAR CAMBIOS DE LA RESERVA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ReservaFormularioViewModel modelo)
    {
        if (id != modelo.IdReserva)
        {
            return NotFound();
        }

        var reserva = await _context.Reservas
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva == null)
        {
            return NotFound();
        }

        // VALIDAR FECHAS
        if (modelo.FechaFin.Date <= modelo.FechaInicio.Date)
        {
            ModelState.AddModelError(
                nameof(modelo.FechaFin),
                "La fecha de salida debe ser posterior al ingreso.");
        }

        // VALIDAR HUÉSPED
        bool existeHuesped = await _context.Huespedes
            .AnyAsync(h => h.IdHuesped == modelo.IdHuesped);

        if (!existeHuesped)
        {
            ModelState.AddModelError(
                nameof(modelo.IdHuesped),
                "Seleccione un huésped válido.");
        }

        // VALIDAR HABITACIÓN
        bool existeHabitacion = await _context.Habitaciones
            .AnyAsync(h => h.IdHabitacion == modelo.IdHabitacion);

        if (!existeHabitacion)
        {
            ModelState.AddModelError(
                nameof(modelo.IdHabitacion),
                "Seleccione una habitación válida.");
        }

        // VALIDAR ESTADO
        if (modelo.Estado != "Pendiente" &&
            modelo.Estado != "Confirmada" &&
            modelo.Estado != "Cancelada")
        {
            ModelState.AddModelError(
                nameof(modelo.Estado),
                "Seleccione un estado válido.");
        }

        // VALIDAR DISPONIBILIDAD
        if (modelo.FechaFin.Date > modelo.FechaInicio.Date &&
            existeHabitacion &&
            modelo.Estado != "Cancelada")
        {
            bool ocupada = await _context.Reservas
                .AnyAsync(r =>
                    r.IdReserva != id &&
                    r.IdHabitacion == modelo.IdHabitacion &&
                    r.Estado != "Cancelada" &&
                    r.FechaInicio < modelo.FechaFin.Date &&
                    modelo.FechaInicio.Date < r.FechaFin);

            if (ocupada)
            {
                ModelState.AddModelError(
                    nameof(modelo.IdHabitacion),
                    "La habitación ya tiene una reserva en esas fechas.");
            }
        }

        // VALIDAR FECHAS DE SERVICIOS CONTRATADOS
        if (modelo.FechaFin.Date > modelo.FechaInicio.Date)
        {
            bool serviciosFueraDeFecha =
                await _context.ReservaServicios.AnyAsync(rs =>
                    rs.IdReserva == id &&
                    (rs.Fecha < modelo.FechaInicio.Date ||
                     rs.Fecha >= modelo.FechaFin.Date));

            if (serviciosFueraDeFecha)
            {
                ModelState.AddModelError(
                    nameof(modelo.FechaFin),
                    "Existen servicios contratados fuera del nuevo período de estadía.");
            }
        }

        // REGRESAR AL FORMULARIO SI HAY ERRORES
        if (!ModelState.IsValid)
        {
            await CargarListasAsync(
                modelo.IdHuesped,
                modelo.IdHabitacion);

            return View(modelo);
        }

        // ACTUALIZAR LOS DATOS
        reserva.IdHuesped = modelo.IdHuesped;
        reserva.IdHabitacion = modelo.IdHabitacion;
        reserva.FechaInicio = modelo.FechaInicio.Date;
        reserva.FechaFin = modelo.FechaFin.Date;
        reserva.MetodoPago = modelo.MetodoPago.Trim();
        reserva.Estado = modelo.Estado;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(
                string.Empty,
                "La reserva fue modificada o eliminada. Vuelva al listado.");

            await CargarListasAsync(
                modelo.IdHuesped,
                modelo.IdHabitacion);

            return View(modelo);
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudieron guardar los cambios.");

            await CargarListasAsync(
                modelo.IdHuesped,
                modelo.IdHabitacion);

            return View(modelo);
        }

        TempData["Exito"] = "Reserva actualizada correctamente.";

        return RedirectToAction(nameof(Index));

    }

    // MOSTRAR DETALLES DE UNA RESERVA
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reserva = await _context.Reservas
            .AsNoTracking()
            .Include(r => r.Huesped)
            .Include(r => r.Habitacion)
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva == null)
        {
            return NotFound();
        }

        return View(reserva);
    }

    // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reserva = await _context.Reservas
            .AsNoTracking()
            .Include(r => r.Huesped)
            .Include(r => r.Habitacion)
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva == null)
        {
            return NotFound();
        }

        bool tieneServicios = await _context.ReservaServicios
            .AnyAsync(rs => rs.IdReserva == reserva.IdReserva);

        ViewData["PuedeEliminar"] =
            reserva.Estado == "Cancelada" && !tieneServicios;

        return View(reserva);
    }

    // CONFIRMAR ELIMINACIÓN
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reserva = await _context.Reservas
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva == null)
        {
            return NotFound();
        }

        // Impedir eliminar reservas no canceladas
        if (reserva.Estado != "Cancelada")
        {
            TempData["Error"] =
                "Solo se pueden eliminar reservas canceladas.";

            return RedirectToAction(nameof(Index));
        }

        // Impedir eliminar reservas con servicios
        bool tieneServicios = await _context.ReservaServicios
            .AnyAsync(rs => rs.IdReserva == id);

        if (tieneServicios)
        {
            TempData["Error"] =
                "No se puede eliminar una reserva que tiene servicios contratados.";

            return RedirectToAction(nameof(Index));
        }

        // Eliminar reserva
        _context.Reservas.Remove(reserva);

        try
        {
            await _context.SaveChangesAsync();

            TempData["Exito"] =
                "Reserva eliminada correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] =
                "No se pudo eliminar la reserva.";
        }

        return RedirectToAction(nameof(Index));
    }



    // CANCELAR UNA RESERVA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        // Buscar la reserva
        var reserva = await _context.Reservas
            .FirstOrDefaultAsync(r => r.IdReserva == id);

        if (reserva == null)
        {
            return NotFound();
        }

        // Comprobar si ya está cancelada
        if (reserva.Estado == "Cancelada")
        {
            TempData["Error"] =
                "Esta reserva ya se encuentra cancelada.";

            return RedirectToAction(
                nameof(Details), new { id });
        }

        // Cambiar el estado
        reserva.Estado = "Cancelada";

        try
        {
            await _context.SaveChangesAsync();

            TempData["Exito"] =
                "Reserva cancelada correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] =
                "No se pudo cancelar la reserva.";
        }

        return RedirectToAction(
            nameof(Details), new { id });
    }


}
