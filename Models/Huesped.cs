using GestionHotelera;

namespace GestionHotelera.Models
{
    public class Huesped
    {
        public int IdHuesped { get; set; }
        public string NombreHuesped { get; set; }
        public string Celular { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}