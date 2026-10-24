using Microsoft.AspNetCore.Components;
using ProyectoWebFutbol.Models.Dominio;

namespace ProyectoWebFutbol.Components.Layout
{
    public partial class Fixture
    {
        [Parameter]
        public List<Partido> Partidos { get; set; } = new ();

        [Parameter]
        public string? TituloJornada { get; set; }

        [Parameter] public EventCallback<Partido> OnPartidoSeleccionado { get; set; }
    }
}

