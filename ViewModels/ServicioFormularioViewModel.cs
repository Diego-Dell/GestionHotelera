using System.ComponentModel.DataAnnotations;
namespace GestionHotelera.ViewModels;
public class ServicioFormularioViewModel
{
    public int IdServicio { get; set; }
    [Required(ErrorMessage = "Ingrese el nombre del servicio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999",
    ParseLimitsInInvariantCulture = true,
    ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }
}
