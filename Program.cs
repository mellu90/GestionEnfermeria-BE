using Microsoft.EntityFrameworkCore;
using GestionEnfermeria.Data; // Ajustado a tu carpeta Data

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 1. CONFIGURACIÓN DE PUERTO (RAILWAY)
// -----------------------------------------------------------------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// -----------------------------------------------------------------------------
// 2. CONFIGURACIÓN DE BASE DE DATOS (PostgreSQL)
// -----------------------------------------------------------------------------
// Intentamos leer la variable de entorno que configuraremos en Railway
var connectionString = Environment.GetEnvironmentVariable("DATABASE");

if (string.IsNullOrEmpty(connectionString))
{
    // Si es local, usamos el nombre exacto de tu appsettings.json
    connectionString = builder.Configuration.GetConnectionString("GestionEnfermeriaContext");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No se encontró la cadena de conexión 'GestionEnfermeriaContext'.");
}

// Registro del contexto usando Npgsql
builder.Services.AddDbContext<GestionEnfermeriaContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// -----------------------------------------------------------------------------
// SWAGGER / CORS
// -----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("myApp", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

// -----------------------------------------------------------------------------
// SERVICIOS
// -----------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

// -----------------------------------------------------------------------------
// MIGRACIÓN AUTOMÁTICA (Indispensable para producción)
// -----------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("--> Intentando aplicar migraciones en Railway...");
        var context = services.GetRequiredService<GestionEnfermeriaContext>();
        context.Database.Migrate();
        logger.LogInformation("--> Migraciones aplicadas correctamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "--> ERROR: No se pudieron aplicar las migraciones.");
    }
}

// -----------------------------------------------------------------------------
// PIPELINE (Middlewares)
// -----------------------------------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("myApp");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "API GestionEnfermeria funcionando! (Conectada a Postgres)");

app.Run();