using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoWebFutbol.Models.Enums;

    namespace ProyectoWebFutbol.Models.DTOs
    {
        [Table("Partido", Schema = "FutbolWeb")]
        public class PartidoDTO
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            public string TorneoId { get; set; } = string.Empty;

            [Required]
            public TipoJornada JornadaRonda { get; set; }

            [Required]
            public string LocalId { get; set; } = string.Empty;

            public int? ScoreLocal { get; set; }
            public int? PenalesLocal { get; set; }

            [Required]
            public string VisitaId { get; set; } = string.Empty;

            public int? ScoreVisita { get; set; }
            public int? PenalesVisita { get; set; }

            [Required]
            public EstadoPartido Estado { get; set; } = EstadoPartido.TBD;

            public TipoFase? FaseTorneo { get; set; }

            public TipoGrupo? GrupoTorneo { get; set; }

            // Propiedades de navegación para relaciones
            [ForeignKey("TorneoId")]
            public virtual TorneoDTO Torneo { get; set; } = null!;

            [ForeignKey("LocalId")]
            public virtual EquipoDTO Local { get; set; } = null!;

            [ForeignKey("VisitaId")]
            public virtual EquipoDTO Visita { get; set; } = null!;
        }
    }