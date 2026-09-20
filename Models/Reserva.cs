using GestionHotelera;
using GestionHotelera.Models;

namespace GestionHotelera.Models
{
    public class Reserva
    {
        public int IdReserva { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public string MetodoPago { get; set; }
        public string Estado { get; set; }

        // Relación con Huesped
        public int IdHuesped { get; set; }
        public Huesped Huesped { get; set; }

        // Relación con Habitacion
        public int IdHabitacion { get; set; }
        public Habitacion Habitacion { get; set; }

        // Relación con servicios
        public ICollection<ReservaServicio> ReservaServicios { get; set; }
    }
}