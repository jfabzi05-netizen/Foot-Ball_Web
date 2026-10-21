using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Models.Dominio
{
    public class Partido
    {
        public int Id { get; set; }
        public MarcadorPartido MarcadorLocal { get; set; } = new MarcadorPartido();
        public MarcadorPartido MarcadorVisita { get; set; } = new MarcadorPartido();
       
        public EstadoPartido Estado { get; set; } = EstadoPartido.TBD;
    }

}
