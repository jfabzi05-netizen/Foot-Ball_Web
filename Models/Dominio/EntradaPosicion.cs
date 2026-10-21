using ProyectoWebFutbol.Models.DTOs;

namespace ProyectoWebFutbol.Models.Dominio
{
    public class EntradaPosicion
    {
        public int Puesto { get; set; }
        public Equipo Equipo { get; set; } = new Equipo();
        public int Puntos { get; set; }
        public int Jugados { get; set; }
        public int Victorias { get; set; }
        public int Empates { get; set;  }
        public int Derrotas { get; set; }
        public int GolesFavor { get; set; }
        public int GolesContra { get; set; }
        public int DiferenciaGoles { get; set; }
        public List<char> UltimosResultados { get; set;  } = new List<char>();
        //D: Empate, W: Victoria, L: Derrota
    }
}
