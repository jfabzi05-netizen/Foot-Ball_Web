using Microsoft.EntityFrameworkCore;
using ProyectoWebFutbol.Data;
using ProyectoWebFutbol.Models.Dominio;
using ProyectoWebFutbol.Models.DTOs;
using ProyectoWebFutbol.Models.Enums;

namespace ProyectoWebFutbol.Servicios
{
    public static class Mappers
    {
        // ==========================================
        // MÉTODOS PRIVADOS DE MAPEO (Mapping Helper)
        // ==========================================

        public static Torneo MapearTorneo(TorneoDTO dto, string storageBaseUrl)
        {
            Torneo torneo = new Torneo();
            torneo.Id = dto.Id;
            torneo.Nombre = dto.Nombre;
            torneo.ColorHex = dto.ColorHex ?? "";
            torneo.UrlImagenLogo = $"{storageBaseUrl}torneos/{dto.Nombre}.png";
            torneo.UrlBackground = $"{storageBaseUrl}torneos/{dto.Nombre}-fondo.png"; torneo.TipoTorneo = dto.TipoTorneo;
            torneo.Index = dto.Index;
            return torneo;
        }

        public static Torneo MapearPartidosTorneo(Torneo torneo, List<PartidoDTO> partidosDto, string storageBaseUrl)
        {
            if (partidosDto == null || !partidosDto.Any())
            {
                return torneo;
            }

            // Agrupación secuencial jerárquica con LINQ
            torneo.Fases = partidosDto
                .GroupBy(p => p.FaseTorneo ?? TipoFase.FaseDeLiga)
                .Select(faseGroup => new FaseTorneo
                {
                    TipoFase = faseGroup.Key,
                    Nombre = ObtenerNombreFase(faseGroup.Key),
                    Grupos = faseGroup
                        .GroupBy(p => p.GrupoTorneo ?? TipoGrupo.General)
                        .Select(grupoGroup => new GrupoTorneo
                        {
                            TipoGrupo = grupoGroup.Key,
                            Nombre = ObtenerNombreGrupo(grupoGroup.Key),
                            HayTabla = grupoGroup.Key == TipoGrupo.General || grupoGroup.Key.ToString().StartsWith("Grupo"),
                            Jornadas = grupoGroup
                                .GroupBy(p => p.JornadaRonda)
                                .Select(jornadaGroup => new JornadaTorneo
                                {
                                    TipoJornada = jornadaGroup.Key,
                                    Nombre = ObtenerNombreJornada(jornadaGroup.Key),
                                    Partidos = jornadaGroup.Select(dto => Mappers.MapearPartido(dto, storageBaseUrl)).OrderBy(p => p.Id).ToList()
                                })
                                .OrderBy(j => j.TipoJornada) // Mantiene el orden secuencial de jornadas
                                .ToList()
                        })
                        .OrderBy(g => g.TipoGrupo)
                        .ToList()
                })
                .OrderBy(f => f.TipoFase)
                .ToList();

            return torneo;
        }

        public static Partido MapearPartido(PartidoDTO dto, string storageBaseUrl)
        {
            Partido partido = new Partido();
            partido.MarcadorLocal.Equipo = Mappers.MapearEquipo(dto.Local, storageBaseUrl);
            if (dto.ScoreLocal != null) partido.MarcadorLocal.Score = dto.ScoreLocal;
            if (dto.PenalesLocal != null) partido.MarcadorLocal.Penales = dto.PenalesLocal;
            partido.MarcadorLocal.Goleadores = dto.GoleadoresLocal;

            partido.MarcadorVisita.Equipo = Mappers.MapearEquipo(dto.Visita, storageBaseUrl);
            if (dto.ScoreVisita != null) partido.MarcadorVisita.Score = dto.ScoreVisita;
            if (dto.PenalesVisita != null) partido.MarcadorVisita.Penales = dto.PenalesVisita;
            partido.MarcadorVisita.Goleadores = dto.GoleadoresVisita;

            partido.Id = dto.Id;
            partido.Estado = dto.Estado;

            return partido;
        }

        public static Equipo MapearEquipo(EquipoDTO dto, string storageBaseUrl)
        {
            Equipo equipo = new Equipo();
            equipo.Id = dto.Id;
            equipo.Nombre = dto.Nombre.ToString();
            equipo.Pais = dto.Pais;
            equipo.UrlImagen = $"{storageBaseUrl}equipos/{dto.Pais}/{dto.Id}.png";
            return equipo;
        }

        public static string ObtenerNombreFase(TipoFase fase) => fase switch
        {
            TipoFase.FaseDeLiga => "Fase de Liga",
            TipoFase.FaseEliminatoria => "Fase Eliminatoria",
            TipoFase.FaseDeGrupos => "Fase de Grupos",
            TipoFase.Apertura => "Apertura",
            TipoFase.Clausura => "Clausura",
            _ => fase.ToString()
        };

        public static string ObtenerNombreGrupo(TipoGrupo grupo) => grupo switch
        {
            TipoGrupo.General => "General",
            TipoGrupo.Rondade16 => "Octavos de Final",
            TipoGrupo.CuartosdeFinal => "Cuartos de Final",
            TipoGrupo.Semifinales => "Semifinales",
            TipoGrupo.TercerPuesto => "Tercer Puesto",
            TipoGrupo.Final => "Final",
            _ => grupo.ToString().Replace("Grupo", "Grupo ")
        };

        public static string ObtenerNombreJornada(TipoJornada jornada) => jornada switch
        {
            TipoJornada.Ida => "Ida",
            TipoJornada.Vuelta => "Vuelta",
            _ => jornada.ToString().Replace("Jornada", "Jornada ")
        };


        public static List<GolItem> ParsearGoleadores(List<string>? goleadores)
        {
            if (goleadores == null || !goleadores.Any()) return new List<GolItem>();

            return goleadores
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Select(GolItem.DesdeString)
                .ToList();
        }

        public static List<string> FormatearGoleadores(List<GolItem>? items)
        {
            if (items == null || !items.Any()) return new List<string>();

            return items
                .Select(g => g.ToString())
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .ToList();
        }
    }
}