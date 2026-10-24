using System.ComponentModel.DataAnnotations;

namespace GestionHotelera.ViewModels;

public class HuespedFormularioViewModel
{
    public int IdHuesped { get; set; }

    [Display(Name = "Nombre del huésped")]
    [Required(ErrorMessage = "Ingrese el nombre del huésped.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    [RegularExpression(
    @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+(?:[ '’-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$",
    ErrorMessage =
        "Ingrese nombres y apellidos usando letras y espacios, sin números ni espacios al inicio o al final.")]
    public string NombreHuesped { get; set; } = string.Empty;

    [Display(Name = "Celular")]
    [Required(ErrorMessage = "Ingrese el número de celular.")]
    [StringLength(
        16,
        ErrorMessage = "El celular no puede superar los 16 caracteres.")]
    [RegularExpression(
        @"^\+?[0-9]{7,15}$",
        ErrorMessage =
            "Ingrese entre 7 y 15 dígitos, con un + opcional al inicio, sin espacios ni guiones.")]
    public string Celular { get; set; } = string.Empty;
}