namespace ProyectoWebFutbol.Models.Dominio
{
    public class ResultadoGlobal
    {
        public MarcadorPartido GlobalLocal { get; set; } = new MarcadorPartido();
        public MarcadorPartido GlobalVisita { get; set; } = new MarcadorPartido();
    }
}
