using ProyectoWebFutbol.Models.DTOs;

namespace ProyectoWebFutbol.Models.Dominio
{
    public class EntradaPosicion
    {
        public int Puesto { get; set; } = 0;
        public Equipo Equipo { get; set; } = new Equipo();
        public int Puntos { get; set; } = 0;
        public int Jugados { get; set; } = 0;
        public int Victorias { get; set; } = 0;
        public int Empates { get; set; } = 0;
        public int Derrotas { get; set; } = 0;
        public int GolesFavor { get; set; } = 0;
        public int GolesContra { get; set; } = 0;
        public int DiferenciaGoles { get; set; } = 0;
        public List<char> UltimosResultados { get; set;  } = new List<char>();
        //D: Empate, W: Victoria, L: Derrota
    }
}
