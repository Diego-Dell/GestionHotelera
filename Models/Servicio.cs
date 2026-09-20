using GestionHotelera;

namespace GestionHotelera.Models
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }

        public ICollection<ReservaServicio> ReservaServicios { get; set; }
    }
}