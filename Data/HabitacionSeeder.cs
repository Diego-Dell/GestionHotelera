
using GestionHotelera.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Data;

public static class HabitacionSeeder
{
    public static async Task InicializarAsync(
        AppDbContext context)
    {
        if (await context.Habitaciones.AnyAsync())
            return;

        var habitaciones = new List<Habitacion>
        {
            new() { Numero = 101, Tipo = "Simple", Precio = 150 },
            new() { Numero = 102, Tipo = "Simple", Precio = 150 },
            new() { Numero = 103, Tipo = "Doble", Precio = 250 },
            new() { Numero = 104, Tipo = "Doble", Precio = 250 },
            new() { Numero = 105, Tipo = "Matrimonial", Precio = 300 },
            new() { Numero = 106, Tipo = "Matrimonial", Precio = 300 },
            new() { Numero = 107, Tipo = "Suite", Precio = 450 },
            new() { Numero = 108, Tipo = "Suite", Precio = 450 },
            new() { Numero = 109, Tipo = "Simple", Precio = 150 },
            new() { Numero = 110, Tipo = "Doble", Precio = 250 },
            new() { Numero = 201, Tipo = "Simple", Precio = 150 },
            new() { Numero = 202, Tipo = "Doble", Precio = 250 },
            new() { Numero = 203, Tipo = "Matrimonial", Precio = 300 },
            new() { Numero = 204, Tipo = "Suite", Precio = 450 },
            new() { Numero = 205, Tipo = "Doble", Precio = 250 }
        };

        context.Habitaciones.AddRange(habitaciones);
        await context.SaveChangesAsync();
    }
}
