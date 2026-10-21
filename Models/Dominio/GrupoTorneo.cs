using ProyectoWebFutbol.Models.DTOs;
using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Models.Dominio
{
    public class GrupoTorneo
    {
        public TipoGrupo TipoGrupo { get; set; }     
        public string Nombre { get; set; } = string.Empty;
        public List<JornadaTorneo> Jornadas { get; set; } = new();
        public Boolean HayTabla { get; set; } = false;
    }

    public class JornadaTorneo
    {
        public TipoJornada TipoJornada { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<Partido> Partidos { get; set; } = new();
    }
}
