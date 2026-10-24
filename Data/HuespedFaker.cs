using Bogus;
using GestionHotelera.Models;

namespace GestionHotelera.Data;

public class HuespedFaker : Faker<Huesped>
{
    public HuespedFaker() : base("es")
    {
        RuleFor(
            h => h.NombreHuesped,
            f => f.Name.FullName());

        RuleFor(
            h => h.Celular,
            f => f.Phone.PhoneNumber("7#######"));
    }
}