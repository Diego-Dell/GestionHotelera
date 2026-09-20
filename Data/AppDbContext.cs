using GestionHotelera.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Huesped> Huespedes { get; set; }
    public DbSet<Habitacion> Habitaciones { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Servicio> Servicios { get; set; }
    public DbSet<ReservaServicio> ReservaServicios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CLAVES PRIMARIAS
        modelBuilder.Entity<Huesped>()
            .HasKey(h => h.IdHuesped);

        modelBuilder.Entity<Habitacion>()
            .HasKey(h => h.IdHabitacion);

        modelBuilder.Entity<Reserva>()
            .HasKey(r => r.IdReserva);

        modelBuilder.Entity<Servicio>()
            .HasKey(s => s.IdServicio);

        // Clave primaria compuesta
        modelBuilder.Entity<ReservaServicio>()
            .HasKey(rs => new
            {
                rs.IdReserva,
                rs.IdServicio
            });

        // HUESPED 1 ---- N RESERVA
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Huesped)
            .WithMany(h => h.Reservas)
            .HasForeignKey(r => r.IdHuesped);

        // HABITACION 1 ---- N RESERVA
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Habitacion)
            .WithMany(h => h.Reservas)
            .HasForeignKey(r => r.IdHabitacion);

        // RESERVA 1 ---- N RESERVASERVICIO
        modelBuilder.Entity<ReservaServicio>()
            .HasOne(rs => rs.Reserva)
            .WithMany(r => r.ReservaServicios)
            .HasForeignKey(rs => rs.IdReserva);

        // SERVICIO 1 ---- N RESERVASERVICIO
        modelBuilder.Entity<ReservaServicio>()
            .HasOne(rs => rs.Servicio)
            .WithMany(s => s.ReservaServicios)
            .HasForeignKey(rs => rs.IdServicio);
    }
}