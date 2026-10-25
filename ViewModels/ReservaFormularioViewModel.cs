
using System.ComponentModel.DataAnnotations;

namespace GestionHotelera.ViewModels;

public class ReservaFormularioViewModel
{
    public int IdReserva { get; set; }

    [Display(Name = "Huésped")]
    [Range(1, int.MaxValue,
        ErrorMessage = "Seleccione un huésped.")]
    public int IdHuesped { get; set; }

    [Display(Name = "Habitación")]
    [Range(1, int.MaxValue,
        ErrorMessage = "Seleccione una habitación.")]
    public int IdHabitacion { get; set; }

    [Display(Name = "Fecha de ingreso")]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Display(Name = "Fecha de salida")]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(1);

    [Display(Name = "Método de pago")]
    [Required(ErrorMessage = "Ingrese el método de pago.")]
    [StringLength(80,
        ErrorMessage = "El método de pago no puede superar 80 caracteres.")]
    public string MetodoPago { get; set; } = string.Empty;

    [Display(Name = "Estado")]
    [Required(ErrorMessage = "Seleccione el estado de la reserva.")]
    public string Estado { get; set; } = "Pendiente";
}
