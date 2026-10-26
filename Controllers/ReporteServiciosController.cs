using GestionHotelera.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Controllers
{
    public class ReporteServiciosController : Controller
    {
        private readonly AppDbContext _context;

        public ReporteServiciosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int? idServicio)
        {
            var query = _context.ReservaServicios
                .Include(rs => rs.Reserva)
                .Include(rs => rs.Servicio)
                .Where(rs => rs.Reserva.Estado != "Cancelada") // Excluir canceladas
                .AsQueryable();

            if (fechaInicio.HasValue)
                query = query.Where(rs => rs.Fecha.Date >= fechaInicio.Value.Date);

            if (fechaFin.HasValue)
                query = query.Where(rs => rs.Fecha.Date <= fechaFin.Value.Date);

            if (idServicio.HasValue)
                query = query.Where(rs => rs.IdServicio == idServicio.Value);

            // Agrupar por servicio para el reporte
            var resultado = await query
                .GroupBy(rs => rs.Servicio.Nombre)
                .Select(g => new
                {
                    Servicio = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewData["IdServicio"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Servicios, "IdServicio", "Nombre", idServicio);

            ViewBag.TotalContrataciones = resultado.Sum(r => r.Cantidad);

            return View(resultado);
        }
    }
}