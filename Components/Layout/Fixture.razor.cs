using Microsoft.AspNetCore.Components;
using ProyectoWebFutbol.Models.Dominio;

namespace ProyectoWebFutbol.Components.Layout
{
    public partial class Fixture
    {
        [Parameter]
        public List<Partido> Partidos { get; set; } = new();

        [Parameter]
        public JornadaTorneo? Jornada { get; set; }

        [Parameter]
        public List<ResultadoGlobal> ResultadosGlobales { get; set; } = new();

        [Parameter]
        public string? TituloJornada { get; set; }

        [Parameter]
        public EventCallback<Partido> OnPartidoSeleccionado { get; set; }

        private ResultadoGlobal? ObtenerGlobalParaPartido(Partido partido)
        {
            if (ResultadosGlobales == null || !ResultadosGlobales.Any() || partido == null)
                return null;

            var local = partido.MarcadorLocal?.Equipo?.Nombre;
            var visita = partido.MarcadorVisita?.Equipo?.Nombre;

            if (string.IsNullOrEmpty(local) || string.IsNullOrEmpty(visita))
                return null;

            // Busca el resultado global correspondiente a los dos equipos
            return ResultadosGlobales.FirstOrDefault(g =>
                (g.GlobalLocal?.Equipo?.Nombre == local && g.GlobalVisita?.Equipo?.Nombre == visita) ||
                (g.GlobalLocal?.Equipo?.Nombre == visita && g.GlobalVisita?.Equipo?.Nombre == local));
        }
    }
}
