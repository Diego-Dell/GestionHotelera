using GestionHotelera.Data;
using GestionHotelera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Controllers
{
    public class ReservaServiciosController : Controller
    {
        private readonly AppDbContext _context;

        public ReservaServiciosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ReservaServicios
        public async Task<IActionResult> Index()
        {
            var reservaServicios = _context.ReservaServicios
                .Include(r => r.Reserva).ThenInclude(r => r.Huesped)
                .Include(r => r.Servicio);
            return View(await reservaServicios.ToListAsync());
        }

        // GET: ReservaServicios/Create
        public IActionResult Create(int? idReserva)
        {
            CargarListas(idReserva, null);
            return View();
        }

        // POST: ReservaServicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReserva,IdServicio,Fecha,Hora")] ReservaServicio reservaServicio)
        {

            ModelState.Remove("Reserva");
            ModelState.Remove("Servicio");


            var reserva = await _context.Reservas.FindAsync(reservaServicio.IdReserva);
            if (reserva != null)
            {
                if (reservaServicio.Fecha.Date < reserva.FechaInicio.Date || reservaServicio.Fecha.Date > reserva.FechaFin.Date)
                {
                    ModelState.AddModelError("Fecha", "La fecha del servicio debe estar dentro de la estadía (entre inicio y fin).");
                }
            }
            else
            {
                ModelState.AddModelError("IdReserva", "La reserva seleccionada no es válida.");
            }

            var existe = await _context.ReservaServicios
                .AnyAsync(rs => rs.IdReserva == reservaServicio.IdReserva && rs.IdServicio == reservaServicio.IdServicio);
            if (existe)
            {
                ModelState.AddModelError("", "Este servicio ya está asignado a esta reserva.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reservaServicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            CargarListas(reservaServicio.IdReserva, reservaServicio.IdServicio);
            return View(reservaServicio);
        }

        public async Task<IActionResult> Delete(int? idReserva, int? idServicio)
        {
            if (idReserva == null || idServicio == null) return NotFound();

            var reservaServicio = await _context.ReservaServicios
                .Include(r => r.Reserva).ThenInclude(r => r.Huesped)
                .Include(r => r.Servicio)
                .FirstOrDefaultAsync(m => m.IdReserva == idReserva && m.IdServicio == idServicio);

            if (reservaServicio == null) return NotFound();

            return View(reservaServicio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idReserva, int idServicio)
        {
            var reservaServicio = await _context.ReservaServicios
                .FirstOrDefaultAsync(m => m.IdReserva == idReserva && m.IdServicio == idServicio);
            if (reservaServicio != null)
            {
                _context.ReservaServicios.Remove(reservaServicio);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void CargarListas(int? idReserva, int? idServicio)
        {
            var reservas = _context.Reservas.Include(r => r.Huesped)
                .Select(r => new {
                    Id = r.IdReserva,
                    Descripcion = r.IdReserva + " - " + r.Huesped.NombreHuesped
                }).ToList();

            ViewData["IdReserva"] = new SelectList(reservas, "Id", "Descripcion", idReserva);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "Nombre", idServicio);
        }
    }
}