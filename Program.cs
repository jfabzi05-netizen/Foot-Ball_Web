using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProyectoWebFutbol.Components;
using ProyectoWebFutbol.Data;
using ProyectoWebFutbol.Models.Enums;
using ProyectoWebFutbol.Servicios;

var builder = WebApplication.CreateBuilder(args);

// 1. Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión.");

// 2. Registrar la lectura de Custom Enums en Npgsql con sus esquemas
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

// Habilitar tipos no mapeados
dataSourceBuilder.EnableUnmappedTypes();
dataSourceBuilder.MapEnum<TipoFase>("FutbolWeb.TipoFase");
dataSourceBuilder.MapEnum<TipoGrupo>("FutbolWeb.TipoGrupo");
dataSourceBuilder.MapEnum<TipoJornada>("FutbolWeb.TipoJornada");
dataSourceBuilder.MapEnum<EstadoPartido>("FutbolWeb.EstadoPartido");

var dataSource = dataSourceBuilder.Build();

// 3. Registrar DbContextFactory pasando el dataSource ya configurado
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddScoped<IDomainService, DomainService>();

// Servicios Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery(); // Ahora encontrará el servicio sin errores
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();