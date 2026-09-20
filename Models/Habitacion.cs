using GestionHotelera;

namespace GestionHotelera.Models
{
    public class Habitacion
    {
        public int IdHabitacion { get; set; }
        public int Numero { get; set; }
        public string Tipo { get; set; }
        public decimal Precio { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}