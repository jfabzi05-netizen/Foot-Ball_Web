using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProyectoWebFutbol.Models.DTOs;
using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EquipoDTO> Equipo => Set<EquipoDTO>();
        public DbSet<TorneoDTO> Torneo => Set<TorneoDTO>();
        public DbSet<PartidoDTO> Partido => Set<PartidoDTO>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("FutbolWeb");

            // Configuración del Enum global para Npgsql
            modelBuilder.HasPostgresEnum<TipoFase>("FutbolWeb", "TipoFase");
            modelBuilder.HasPostgresEnum<TipoGrupo>("FutbolWeb", "TipoGrupo");
            modelBuilder.HasPostgresEnum<TipoJornada>("FutbolWeb", "TipoJornada");
            modelBuilder.HasPostgresEnum<EstadoPartido>("FutbolWeb", "EstadoPartido");

            // Relaciones de PartidoDTO
            modelBuilder.Entity<PartidoDTO>()
                .HasOne(p => p.Local)
                .WithMany()
                .HasForeignKey(p => p.LocalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PartidoDTO>()
                .HasOne(p => p.Visita)
                .WithMany()
                .HasForeignKey(p => p.VisitaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PartidoDTO>()
                .HasOne(p => p.Torneo)
                .WithMany()
                .HasForeignKey(p => p.TorneoId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================================================
            // CONFIGURACIÓN DE PARTIDODTO
            // =======================================================
            modelBuilder.Entity<PartidoDTO>(entity =>
            {
                entity.ToTable("Partido", "FutbolWeb");

                entity.Property(p => p.FaseTorneo)
                      .HasConversion(
                          v => v == TipoFase.FaseDeLiga ? "Fase de Liga" :
                               v == TipoFase.FaseEliminatoria ? "Fase Eliminatoria" :
                               v == TipoFase.FaseDeGrupos ? "Fase de Grupos" :
                               v.ToString(),
                          v => v == "Fase de Liga" ? TipoFase.FaseDeLiga :
                               v == "Fase Eliminatoria" ? TipoFase.FaseEliminatoria :
                               v == "Fase de Grupos" ? TipoFase.FaseDeGrupos :
                               Enum.Parse<TipoFase>(v)
                      );

                entity.Property(p => p.GrupoTorneo)
                      .HasConversion(
                          v => v.ToString(),
                          v => Enum.Parse<TipoGrupo>(v.Replace(" ", ""))
                      );

                entity.Property(p => p.JornadaRonda)
                      .HasConversion(
                          v => v.ToString(),
                          v => Enum.Parse<TipoJornada>(v.Replace(" ", ""))
                      );

                // CONFIGURACIÓN DE ESTADO DE PARTIDO
                entity.Property(p => p.Estado)
                      .HasColumnName("Estado")
                      .HasConversion(
                          v => v.ToString(),
                          v => Enum.Parse<EstadoPartido>(v)
                      );
            });

            // TorneoDTO
            modelBuilder.Entity<TorneoDTO>()
                .Property(t => t.TipoTorneo)
                .HasConversion<string>();
        }
    }
}