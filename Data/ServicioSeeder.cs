
using GestionHotelera.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Data;

public static class ServicioSeeder
{
    public static async Task InicializarAsync(
        AppDbContext context)
    {
        if (await context.Servicios.AnyAsync())
            return;

        var servicios = new List<Servicio>
        {
            new() { Nombre = "Desayuno", Precio = 35 },
            new() { Nombre = "Almuerzo", Precio = 50 },
            new() { Nombre = "Cena", Precio = 50 },
            new() { Nombre = "Lavandería", Precio = 40 },
            new() { Nombre = "Transporte", Precio = 80 },
            new() { Nombre = "Estacionamiento", Precio = 25 },
            new() { Nombre = "Spa", Precio = 120 },
            new() { Nombre = "Masajes", Precio = 100 },
            new() { Nombre = "Servicio a la habitación", Precio = 30 },
            new() { Nombre = "Internet Premium", Precio = 20 },
            new() { Nombre = "Gimnasio", Precio = 35 },
            new() { Nombre = "Piscina", Precio = 40 }
        };

        context.Servicios.AddRange(servicios);
        await context.SaveChangesAsync();
    }
}
