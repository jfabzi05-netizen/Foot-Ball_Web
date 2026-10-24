using Microsoft.AspNetCore.Components;
using ProyectoWebFutbol.Models.Dominio;
using ProyectoWebFutbol.Models.Enums;
using ProyectoWebFutbol.Servicios;

namespace ProyectoWebFutbol.Components.Layout
{
    public partial class MatchCard
    {
        [Parameter, EditorRequired]
        public Partido? Partido { get; set; }

        private bool EsGanadorLocal =>
            (Partido?.MarcadorLocal.Score ?? -1) >= (Partido?.MarcadorVisita.Score ?? -1);

        private bool EsGanadorVisita =>
            (Partido?.MarcadorVisita.Score ?? -1) >= (Partido?.MarcadorLocal.Score ?? -1);

        //EDITAR MARCADORES

        private void CambiarScoreLocal(ChangeEventArgs e)
        {
            if (Partido != null)
            {
                string? valor = e.Value?.ToString();

                // Si el usuario borra todo el campo (texto vacío), se asigna null
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Partido.MarcadorLocal.Score = null;
                }
                else if (int.TryParse(valor, out int nuevoScore))
                {
                    Partido.MarcadorLocal.Score = nuevoScore;
                }
            }
        }

        private void CambiarScoreVisita(ChangeEventArgs e)
        {
            if (Partido != null)
            {
                string? valor = e.Value?.ToString();

                // Si el usuario borra todo el campo (texto vacío), se asigna null
                if (string.IsNullOrWhiteSpace(valor))
                {
                    Partido.MarcadorVisita.Score = null;
                }
                else if (int.TryParse(valor, out int nuevoScore))
                {
                    Partido.MarcadorVisita.Score = nuevoScore;
                }
            }
        }

        private async Task GuardarCambiosAsync()
        {
            if (Partido != null)
            {
                // Envía el valor (que puede ser un int o null) a la base de datos
                await DomainService.ActualizarMarcadorAsync(
                    Partido.Id,
                    Partido.MarcadorLocal.Score,
                    Partido.MarcadorVisita.Score,
                    Partido.Estado
                );
            }
        }

        private string ObtenerClaseEstado()
        {
            var estado = Partido?.Estado.ToString().ToLower() ?? "";
            if (estado.Contains("en vivo") || estado == "live") return "live";
            if (estado.Contains("final") || estado == "ft") return "ft";
            return "tbd";
        }

        private string ObtenerTextoEstado()
        {
            if (Partido == null)
                return "TBD";

            var estado = Partido.Estado.ToString().ToLower();

            if (estado.Contains("final") || estado == "ft") return "FT";
            if (estado.Contains("en vivo") || estado == "live") return "LIVE";

            return Partido.Estado.ToString().ToUpper();
        }

        private async Task RotarEstadoAsync()
        {
            if (Partido == null) return;

            // Cambiar usando la propiedad del Enum directamente
            switch (Partido.Estado)
            {
                case EstadoPartido.TBD:
                    Partido.Estado = EstadoPartido.LIVE;
                    break;

                case EstadoPartido.LIVE:
                    Partido.Estado = EstadoPartido.FT;
                    break;

                case EstadoPartido.FT:
                default:
                    Partido.Estado = EstadoPartido.TBD;
                    break;
            }

            await GuardarCambiosAsync();
        }

        [Parameter] public EventCallback<Partido> OnPartidoSeleccionado { get; set; }

        private async Task SeleccionarPartido()
        {
            if (Partido != null && OnPartidoSeleccionado.HasDelegate)
            {
                await OnPartidoSeleccionado.InvokeAsync(Partido);
            }
        }
    }
}
