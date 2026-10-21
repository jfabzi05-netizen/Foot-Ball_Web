using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Models.Dominio
{
    public class FaseTorneo
    {
        public TipoFase TipoFase { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<GrupoTorneo> Grupos { get; set; } = new List<GrupoTorneo>();
    }
}
