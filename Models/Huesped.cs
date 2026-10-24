namespace GestionHotelera.Models;

public class Huesped
{
    public int IdHuesped { get; set; }

    public string NombreHuesped { get; set; } = string.Empty;

    public string Celular { get; set; } = string.Empty;

    public ICollection<Reserva> Reservas { get; set; }
        = new List<Reserva>();
}