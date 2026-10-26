using GestionHotelera.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Controllers
{
    public class ReporteOcupacionController : Controller
    {
        private readonly AppDbContext _context;

        public ReporteOcupacionController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fechaFiltro, string tipoFiltro)
        {
            var fecha = fechaFiltro ?? DateTime.Today;

            var habitacionesQuery = _context.Habitaciones.AsQueryable();

            if (!string.IsNullOrEmpty(tipoFiltro))
            {
                habitacionesQuery = habitacionesQuery.Where(h => h.Tipo == tipoFiltro);
            }

            var habitaciones = await habitacionesQuery.ToListAsync();

            // Buscar qué habitaciones están ocupadas en esa fecha
            var ocupadasIds = await _context.Reservas
                .Where(r => r.Estado != "Cancelada" && r.FechaInicio.Date <= fecha.Date && r.FechaFin.Date >= fecha.Date)
                .Select(r => r.IdHabitacion)
                .ToListAsync();

            ViewBag.Fecha = fecha.ToString("yyyy-MM-dd");
            ViewBag.TipoFiltro = tipoFiltro;
            ViewBag.Total = habitaciones.Count;
            ViewBag.Ocupadas = habitaciones.Count(h => ocupadasIds.Contains(h.IdHabitacion));
            ViewBag.Disponibles = (int)ViewBag.Total - (int)ViewBag.Ocupadas;

            // Pasar la lista para mostrar detalles en la vista
            ViewBag.ListaHabitaciones = habitaciones;
            ViewBag.ListaOcupadasIds = ocupadasIds;

            return View();
        }
    }
}