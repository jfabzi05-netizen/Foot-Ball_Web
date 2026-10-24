using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using ProyectoWebFutbol.Components.Pages;
using ProyectoWebFutbol.Data;
using ProyectoWebFutbol.Models.Dominio;
using ProyectoWebFutbol.Models.DTOs;
using ProyectoWebFutbol.Models.Enums;
using System.ComponentModel;
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
            return dtos.Select(dto => Mappers.MapearTorneo(dto, _storageBaseUrl)).OrderBy(t=> t.Index).ToList();
        }

        public async Task<Torneo> ObtenerTorneoPorIdAsync(string id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var torneodto = await context.Torneo
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id) ?? new TorneoDTO();

            var torneo = Mappers.MapearTorneo(torneodto, _storageBaseUrl);

            // Agregamos los Include para cargar las entidades de EquipoLocal y EquipoVisita
            var partidosdto = await context.Partido
                .Include(p => p.Local)
                .Include(p => p.Visita)
                .AsNoTracking()
                .Where(e => e.TorneoId == id)
                .ToListAsync();

            return Mappers.MapearPartidosTorneo(torneo, partidosdto,_storageBaseUrl);
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

        public async Task ActualizarGoleadoresPartidoAsync(int partidoId, List<string> goleadoresLocal, List<string> goleadoresVisita)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var partido = await context.Partido.FindAsync(partidoId);
            if (partido != null)
            {
                partido.GoleadoresLocal = goleadoresLocal;
                partido.GoleadoresVisita = goleadoresVisita;

                await context.SaveChangesAsync();
            }
        }

        

        public async Task<List<EntradaPosicion>> CalcularTablaPosicionesAsync(List<JornadaTorneo> jornadas)
        {
            if (jornadas == null || !jornadas.Any())
                return new List<EntradaPosicion>();

            // Crear las entradas para los equipos (Toma dos jornadas de prueba)
            var equipos = jornadas
                .Take(3) // Toma los objetos de las 3 primeras jornadas
                .SelectMany(j => j.Partidos) // Obtiene todos los partidos de esas 3 jornadas
                .SelectMany(p => new[] { p.MarcadorLocal.Equipo, p.MarcadorVisita.Equipo }) // Obtiene local y visitante
                .Distinct() // Elimina duplicados
                .ToList();


            // Diccionario auxiliar para acumular las estadísticas por equipo
            var tablaDict = new Dictionary<string, EntradaPosicion>();

            foreach (Equipo equipo in equipos)
            {
                tablaDict[equipo.Id] = new EntradaPosicion { Equipo = equipo };
            }

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