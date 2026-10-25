using System.ComponentModel.DataAnnotations;
namespace GestionHotelera.ViewModels;
public class HabitacionFormularioViewModel
{
    public int IdHabitacion { get; set; }
    [Display(Name = "Número")]
    [Range(1, int.MaxValue, ErrorMessage = "Ingrese un número de habitación válido.")]
    public int Numero { get; set; }

    [Required(ErrorMessage = "Ingrese el tipo de habitación.")]
    [StringLength(60, ErrorMessage = "El tipo no puede superar 60 caracteres.")]
    public string Tipo { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999",
    ParseLimitsInInvariantCulture = true,
    ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }
}
