using ProyectoWebFutbol.Models.DTOs;

namespace ProyectoWebFutbol.Models.Dominio
{
    public class MarcadorPartido
    {
        public Equipo Equipo = new Equipo();
        public int? Score;
        public int? Penales;
        public List<string>? Goleadores { get; set; } = new();
    }
}
