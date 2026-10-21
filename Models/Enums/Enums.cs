namespace ProyectoWebFutbol.Models.Enums
{
    using NpgsqlTypes;

        public enum TipoFase
        {
            [PgName("Fase de Liga")]
            FaseDeLiga,

            [PgName("Fase Eliminatoria")]
            FaseEliminatoria,

            [PgName("Fase de Grupos")]
            FaseDeGrupos,

            [PgName("Apertura")]
            Apertura,

            [PgName("Clausura")]
            Clausura
        }

        public enum TipoGrupo
        {
            [PgName("Grupo A")]
            GrupoA,

            [PgName("Grupo B")]
            GrupoB,

            [PgName("Grupo C")]
            GrupoC,

            [PgName("Grupo D")]
            GrupoD,

            [PgName("Grupo E")]
            GrupoE,

            [PgName("Grupo F")]
            GrupoF,

            [PgName("Grupo G")]
            GrupoG,

            [PgName("Grupo H")]
            GrupoH,

            [PgName("Grupo I")]
            GrupoI,

            [PgName("Grupo J")]
            GrupoJ,

            [PgName("Grupo K")]
            GrupoK,

            [PgName("Grupo L")]
            GrupoL,

            [PgName("General")]
            General,

            [PgName("Ronda de 16")]
            RondaDe16,

            [PgName("Cuartos de Final")]
            CuartosDeFinal,

            [PgName("Semifinales")]
            Semifinales,

            [PgName("Tercer Puesto")]
            TercerPuesto,

            [PgName("Final")]
            Final
        }

        public enum TipoJornada
        {
            [PgName("Ida")]
            Ida,

            [PgName("Vuelta")]
            Vuelta,

            [PgName("Jornada 1")]
            Jornada1,

            [PgName("Jornada 2")]
            Jornada2,

            [PgName("Jornada 3")]
            Jornada3,

            [PgName("Jornada 4")]
            Jornada4,

            [PgName("Jornada 5")]
            Jornada5,

            [PgName("Jornada 6")]
            Jornada6,

            [PgName("Jornada 7")]
            Jornada7,

            [PgName("Jornada 8")]
            Jornada8,

            [PgName("Jornada 9")]
            Jornada9,

            [PgName("Jornada 10")]
            Jornada10,

            [PgName("Jornada 11")]
            Jornada11,

            [PgName("Jornada 12")]
            Jornada12,

            [PgName("Jornada 13")]
            Jornada13,

            [PgName("Jornada 14")]
            Jornada14,

            [PgName("Jornada 15")]
            Jornada15,

            [PgName("Jornada 16")]
            Jornada16,

            [PgName("Jornada 17")]
            Jornada17,

            [PgName("Jornada 18")]
            Jornada18,

            [PgName("Jornada 19")]
            Jornada19,

            [PgName("Jornada 20")]
            Jornada20,

            [PgName("Jornada 21")]
            Jornada21,

            [PgName("Jornada 22")]
            Jornada22,

            [PgName("Jornada 23")]
            Jornada23,

            [PgName("Jornada 24")]
            Jornada24,

            [PgName("Jornada 25")]
            Jornada25,

            [PgName("Jornada 26")]
            Jornada26,

            [PgName("Jornada 27")]
            Jornada27,

            [PgName("Jornada 28")]
            Jornada28,

            [PgName("Jornada 29")]
            Jornada29,

            [PgName("Jornada 30")]
            Jornada30,

            [PgName("Jornada 31")]
            Jornada31,

            [PgName("Jornada 32")]
            Jornada32,

            [PgName("Jornada 33")]
            Jornada33,

            [PgName("Jornada 34")]
            Jornada34,

            [PgName("Jornada 35")]
            Jornada35,

            [PgName("Jornada 36")]
            Jornada36,

            [PgName("Jornada 37")]
            Jornada37,

            [PgName("Jornada 38")]
            Jornada38
        }
    
        public enum TipoTorneo
        {
        Nacional,
        Internacional
        // Agrega los valores exactos que tienes en Postgres
   }
    [PgName("EstadoPartido")] // Nombre del enum en PostgreSQL
    public enum EstadoPartido
    {
        [PgName("TBD")]
        TBD,

        [PgName("LIVE")]
        LIVE,

        [PgName("FT")]
        FT
    }
}

