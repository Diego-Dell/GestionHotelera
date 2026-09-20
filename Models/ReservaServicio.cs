using GestionHotelera.Models;

namespace GestionHotelera.Models
{
    public class ReservaServicio
    {
        public int IdReserva { get; set; }
        public Reserva Reserva { get; set; }

        public int IdServicio { get; set; }
        public Servicio Servicio { get; set; }

        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }
}