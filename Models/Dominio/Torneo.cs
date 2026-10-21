using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Models.Dominio
{
    public class Torneo
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ColorHex {  get; set; } = string.Empty;
        public List<FaseTorneo> Fases { get; set; } = new List<FaseTorneo>();
        public TipoTorneo TipoTorneo { get; set; }
        public string UrlImagenLogo { get; set; } = string.Empty;
        public string UrlBackground { get; set; } = string.Empty;
    }
}
