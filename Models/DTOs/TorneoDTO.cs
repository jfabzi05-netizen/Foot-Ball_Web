using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Models.DTOs
{
    public class TorneoDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? ColorHex { get; set;}
        public TipoTorneo TipoTorneo { get; set; } 
    }
}
