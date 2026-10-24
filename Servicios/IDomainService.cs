using ProyectoWebFutbol.Models.DTOs;
using ProyectoWebFutbol.Models.Dominio;
using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Servicios
{
    public interface IDomainService
    {
        Task<List<Torneo>> ObtenerTorneosAsync();
        Task<Torneo> ObtenerTorneoPorIdAsync(string id);
        Task ActualizarMarcadorAsync(int partidoId, int? scoreLocal, int? scoreVisita, EstadoPartido estado);
        Task<List<EntradaPosicion>> CalcularTablaPosicionesAsync(List<JornadaTorneo> jornadas);
        Task ActualizarGoleadoresPartidoAsync(int partidoId, List<string> goleadoresLocal, List<string> goleadoresVisita);

    }
}