using Microsoft.AspNetCore.Components;
using ProyectoWebFutbol.Models;
using ProyectoWebFutbol.Models.Dominio;
using ProyectoWebFutbol.Servicios;

namespace ProyectoWebFutbol.Components.Layout
{
    public partial class MatchDetail
    {
        [Parameter] public bool Mostrar { get; set; }
        [Parameter] public Partido? Partido { get; set; }
        [Parameter] public EventCallback<bool> MostrarChanged { get; set; }
        [Parameter] public EventCallback OnGuardado { get; set; }

        private bool esModoEdicion = false;
        private bool cargando = false;
        private List<GolItem> golesLocal = new();
        private List<GolItem> golesVisita = new();

        // Retorna true solo si las listas existen y contienen al menos un gol con datos
        private bool TieneGolesRegistrados =>
            golesLocal != null &&
            golesVisita != null &&
            (golesLocal.Any(g => !string.IsNullOrWhiteSpace(g.Jugador) || !string.IsNullOrWhiteSpace(g.Minuto)) ||
             golesVisita.Any(g => !string.IsNullOrWhiteSpace(g.Jugador) || !string.IsNullOrWhiteSpace(g.Minuto)));
        protected override void OnParametersSet()
        {
            if (Mostrar && Partido != null)
            {
                esModoEdicion = false;
                CargarGoles();
            }
        }

        private void CargarGoles()
        {
            if (Partido == null) return;
            golesLocal = Mappers.ParsearGoleadores(Partido.MarcadorLocal.Goleadores);
            golesVisita = Mappers.ParsearGoleadores(Partido.MarcadorVisita.Goleadores);
        }

        private void ActivarModoEdicion()
        {
            if (Partido == null) return;

            int scoreLocal = Partido.MarcadorLocal.Score ?? 0;
            int scoreVisita = Partido.MarcadorVisita.Score ?? 0;

            while (golesLocal.Count < scoreLocal)
            {
                golesLocal.Add(new GolItem());
            }

            while (golesVisita.Count < scoreVisita)
            {
                golesVisita.Add(new GolItem());
            }

            // Si el marcador disminuyó, recortar los elementos sobrantes
            if (golesLocal.Count > scoreLocal)
                golesLocal = golesLocal.Take(scoreLocal).ToList();

            if (golesVisita.Count > scoreVisita)
                golesVisita = golesVisita.Take(scoreVisita).ToList();

            esModoEdicion = true;
        }

        private async Task Cerrar()
        {
            Mostrar = false;
            esModoEdicion = false;
            await MostrarChanged.InvokeAsync(false);
        }

        private async Task GuardarYVolverAsync()
        {
            if (Partido == null) return;

            cargando = true;
            try
            {
                var listaLocal = Mappers.FormatearGoleadores(golesLocal);
                var listaVisita = Mappers.FormatearGoleadores(golesVisita);

                Partido.MarcadorLocal.Goleadores = listaLocal;
                Partido.MarcadorVisita.Goleadores = listaVisita;

                await DomainService.ActualizarGoleadoresPartidoAsync(Partido.Id, listaLocal, listaVisita);
                await OnGuardado.InvokeAsync();

                CargarGoles();
                esModoEdicion = false;
            }
            finally
            {
                cargando = false;
            }
        }
    }
}