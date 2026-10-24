using Microsoft.EntityFrameworkCore;

namespace GestionHotelera.Models;

public class ListaPaginada<T> : List<T>
{
    public int PaginaActual { get; }

    public int TotalPaginas { get; }

    public int TotalRegistros { get; }

    public bool TieneAnterior => PaginaActual > 1;

    public bool TieneSiguiente => PaginaActual < TotalPaginas;

    public ListaPaginada(
        List<T> elementos,
        int totalRegistros,
        int paginaActual,
        int totalPaginas)
    {
        AddRange(elementos);

        TotalRegistros = totalRegistros;
        PaginaActual = paginaActual;
        TotalPaginas = totalPaginas;
    }

    public static async Task<ListaPaginada<T>> CrearAsync(
        IOrderedQueryable<T> consulta,
        int pagina,
        int tamanoPagina)
    {
        if (tamanoPagina <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tamanoPagina));
        }

        int totalRegistros = await consulta.CountAsync();

        int totalPaginas = Math.Max(
            1,
            (int)Math.Ceiling(totalRegistros / (double)tamanoPagina));

        pagina = Math.Clamp(pagina, 1, totalPaginas);

        var elementos = await consulta
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return new ListaPaginada<T>(
            elementos,
            totalRegistros,
            pagina,
            totalPaginas);
    }
}