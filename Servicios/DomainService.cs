using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using ProyectoWebFutbol.Components.Pages;
using ProyectoWebFutbol.Data;
using ProyectoWebFutbol.Models.Dominio;
using ProyectoWebFutbol.Models.DTOs;
using ProyectoWebFutbol.Models.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace ProyectoWebFutbol.Servicios
{
    public class DomainService : IDomainService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly string _storageBaseUrl;

        public DomainService(IDbContextFactory<ApplicationDbContext> dbFactory,
            IConfiguration configuration)
        {
            _dbFactory = dbFactory;
            _storageBaseUrl = configuration["SupabaseStorageBaseUrl"] ?? "";
        }
        //IMPLEMENTACIÓN:
        public async Task<List<Torneo>> ObtenerTorneosAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var dtos = await context.Torneo.ToListAsync(); 
            return dtos.Select(MapearTorneo).OrderBy(t=> t.Nombre).ToList();
        }

        public async Task<Torneo> ObtenerTorneoPorIdAsync(string id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var torneodto = await context.Torneo
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id) ?? new TorneoDTO();

            var torneo = MapearTorneo(torneodto);

            // Agregamos los Include para cargar las entidades de EquipoLocal y EquipoVisita
            var partidosdto = await context.Partido
                .Include(p => p.Local)
                .Include(p => p.Visita)
                .AsNoTracking()
                .Where(e => e.TorneoId == id)
                .ToListAsync();

            return MapearPartidosTorneo(torneo, partidosdto);
        }

        public async Task ActualizarMarcadorAsync(int partidoId, int? scoreLocal, int? scoreVisita, EstadoPartido estado)
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var partido = await context.Partido.FindAsync(partidoId);
            if (partido != null)
            {
                partido.Estado = estado;
                partido.ScoreLocal = scoreLocal;
                partido.ScoreVisita = scoreVisita;
                // Forzamos el casteo explícito del enum en PostgreSQL (::"FutbolWeb"."EstadoPartido")
                string estadoTexto = estado.ToString();
                await context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE \"FutbolWeb\".\"Partido\" SET \"ScoreLocal\" = {scoreLocal}, \"ScoreVisita\" = {scoreVisita}, \"Estado\" = {estadoTexto}::\"FutbolWeb\".\"EstadoPartido\" WHERE \"Id\" = {partidoId}");
            }
        }

        

        // ==========================================
        // MÉTODOS PRIVADOS DE MAPEO (Mapping Helper)
        // ==========================================

        private Torneo MapearTorneo(TorneoDTO dto)
        {
            Torneo torneo = new Torneo();
            torneo.Id = dto.Id;
            torneo.Nombre = dto.Nombre;
            torneo.ColorHex = dto.ColorHex ?? "";
            torneo.UrlImagenLogo = _storageBaseUrl + "torneos/" + dto.Nombre + ".png";
            torneo.UrlBackground = _storageBaseUrl + "torneos/" + dto.Nombre + "-fondo.png";
            torneo.TipoTorneo = dto.TipoTorneo;
            return torneo;
        }

        private Torneo MapearPartidosTorneo(Torneo torneo, List<PartidoDTO> partidosDto)
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
                                    Partidos = jornadaGroup.Select(MapearPartido).OrderBy(p => p.Id).ToList()
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

        private Partido MapearPartido (PartidoDTO dto)
        {
            Partido partido = new Partido();
            partido.MarcadorLocal.Equipo = MapearEquipo(dto.Local);
            if (dto.ScoreLocal != null) partido.MarcadorLocal.Score = dto.ScoreLocal;
            if (dto.PenalesLocal != null) partido.MarcadorLocal.Penales = dto.PenalesLocal;

            partido.MarcadorVisita.Equipo = MapearEquipo(dto.Visita);
            if (dto.ScoreVisita != null) partido.MarcadorVisita.Score = dto.ScoreVisita;
            if (dto.PenalesVisita != null) partido.MarcadorVisita.Penales = dto.PenalesVisita;


            partido.Id = dto.Id;
            partido.Estado = dto.Estado;

            return partido;
        }

        private Equipo MapearEquipo(EquipoDTO dto)
        {
            Equipo equipo = new Equipo();
            equipo.Id = dto.Id;
            equipo.Nombre = dto.Nombre.ToString();
            equipo.Pais = dto.Pais;
            equipo.UrlImagen = _storageBaseUrl + "equipos/" +dto.Pais+"/"+ dto.Id + ".png";
            return equipo;
        }

        private string ObtenerNombreFase(TipoFase fase) => fase switch
        {
            TipoFase.FaseDeLiga => "Fase de Liga",
            TipoFase.FaseEliminatoria => "Fase Eliminatoria",
            TipoFase.FaseDeGrupos => "Fase de Grupos",
            TipoFase.Apertura => "Apertura",
            TipoFase.Clausura => "Clausura",
            _ => fase.ToString()
        };

        private string ObtenerNombreGrupo(TipoGrupo grupo) => grupo switch
        {
            TipoGrupo.General => "General",
            TipoGrupo.RondaDe16 => "Octavos de Final",
            TipoGrupo.CuartosDeFinal => "Cuartos de Final",
            TipoGrupo.Semifinales => "Semifinales",
            TipoGrupo.TercerPuesto => "Tercer Puesto",
            TipoGrupo.Final => "Final",
            _ => grupo.ToString().Replace("Grupo", "Grupo ")
        };

        private string ObtenerNombreJornada(TipoJornada jornada) => jornada switch
        {
            TipoJornada.Ida => "Ida",
            TipoJornada.Vuelta => "Vuelta",
            _ => jornada.ToString().Replace("Jornada", "Jornada ")
        };

        public async Task<List<EntradaPosicion>> CalcularTablaPosicionesAsync(List<JornadaTorneo> jornadas)
        {
            if (jornadas == null || !jornadas.Any())
                return new List<EntradaPosicion>();

            // Diccionario auxiliar para acumular las estadísticas por equipo
            var tablaDict = new Dictionary<string, EntradaPosicion>();

            // Obtener todos los partidos de todas las jornadas recibidas
            var partidos = jornadas
                .SelectMany(j => j.Partidos)
                .Where(p => p.Estado == EstadoPartido.FT || p.Estado == EstadoPartido.LIVE) // Solo partidos finalizados
                .OrderBy(p => p.Id) // Orden cronológico
                .ToList();

            foreach (var partido in partidos)
            {
                var local = partido.MarcadorLocal.Equipo;
                var visita = partido.MarcadorVisita.Equipo;

                if (local == null || visita == null) continue;

                // Registrar/obtener entrada de la tabla para ambos equipos
                if (!tablaDict.ContainsKey(local.Id))
                    tablaDict[local.Id] = new EntradaPosicion { Equipo = local };

                if (!tablaDict.ContainsKey(visita.Id))
                    tablaDict[visita.Id] = new EntradaPosicion { Equipo = visita };

                var entradaLocal = tablaDict[local.Id];
                var entradaVisita = tablaDict[visita.Id];

                int scoreLocal = partido.MarcadorLocal.Score ?? 0;
                int scoreVisita = partido.MarcadorVisita.Score ?? 0;

                // Actualizar datos de jugados y goles
                entradaLocal.Jugados++;
                entradaLocal.GolesFavor += scoreLocal;
                entradaLocal.GolesContra += scoreVisita;

                entradaVisita.Jugados++;
                entradaVisita.GolesFavor += scoreVisita;
                entradaVisita.GolesContra += scoreLocal;

                // Determinar resultado del partido
                if (scoreLocal > scoreVisita)
                {
                    // Victoria Local
                    entradaLocal.Victorias++;
                    entradaLocal.Puntos += 3;
                    entradaLocal.UltimosResultados.Add('W');

                    entradaVisita.Derrotas++;
                    entradaVisita.UltimosResultados.Add('L');
                }
                else if (scoreLocal < scoreVisita)
                {
                    // Victoria Visitante
                    entradaVisita.Victorias++;
                    entradaVisita.Puntos += 3;
                    entradaVisita.UltimosResultados.Add('W');

                    entradaLocal.Derrotas++;
                    entradaLocal.UltimosResultados.Add('L');
                }
                else
                {
                    // Empate
                    entradaLocal.Empates++;
                    entradaLocal.Puntos += 1;
                    entradaLocal.UltimosResultados.Add('D');

                    entradaVisita.Empates++;
                    entradaVisita.Puntos += 1;
                    entradaVisita.UltimosResultados.Add('D');
                }
            }

            // Calcular diferencia de goles y ordenar la tabla según las reglas estándar de fútbol
            var tablaOrdenada = tablaDict.Values
                .Select(e =>
                {
                    e.DiferenciaGoles = e.GolesFavor - e.GolesContra;
                    // Limitar la racha a los últimos 5 partidos
                    if (e.UltimosResultados.Count > 5)
                    {
                        e.UltimosResultados = e.UltimosResultados.TakeLast(5).ToList();
                    }
                    return e;
                })
                .OrderByDescending(e => e.Puntos)
                .ThenByDescending(e => e.DiferenciaGoles)
                .ThenByDescending(e => e.GolesFavor)
                .ThenBy(e => e.Equipo.Nombre)
                .ToList();

            // Asignar el puesto en la tabla
            for (int i = 0; i < tablaOrdenada.Count; i++)
            {
                tablaOrdenada[i].Puesto = i + 1;
            }

            return await Task.FromResult(tablaOrdenada);
        }
    }
}