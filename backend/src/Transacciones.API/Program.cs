using Microsoft.EntityFrameworkCore;
using Transacciones.API.Clients;
using Transacciones.API.Data;
using Transacciones.API.Repositories;
using Transacciones.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5432;Database=inventario_db;Username=postgres;Password=postgrespassword";

builder.Services.AddDbContext<TransaccionesDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configuración de Cliente REST Síncrono hacia Productos.API
var productosApiUrl = builder.Configuration["Services:ProductosApiUrl"] ?? "http://localhost:5001/";
builder.Services.AddHttpClient<IProductoRestClient, ProductoRestClient>(client =>
{
    client.BaseAddress = new Uri(productosApiUrl);
});

// Inyección de dependencias (SOLID - DIP)
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para comunicación con el Frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();

// Redireccionar la raíz "/" a Swagger UI automáticamente
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Run();
