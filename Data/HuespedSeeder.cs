using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Data;

public static class HuespedSeeder
{
    public static async Task InicializarAsync(
        AppDbContext context,
        int cantidad)
    {
        if (cantidad <= 0)
        {
            return;
        }

        if (await context.Huespedes.AnyAsync())
        {
            return;
        }

        var faker = new HuespedFaker();

        context.Huespedes.AddRange(faker.Generate(cantidad));

        await context.SaveChangesAsync();
    }
}

//Funcionamiento: si la tabla está vacía, agrega datos; si ya contiene huéspedes, no agrega más.