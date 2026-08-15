using Microsoft.EntityFrameworkCore;
using Productos.API.Data;
using Productos.API.Repositories;
using Productos.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5432;Database=inventario_db;Username=postgres;Password=postgrespassword";

builder.Services.AddDbContext<ProductosDbContext>(options =>
    options.UseNpgsql(connectionString));

// Inyección de dependencias (SOLID - DIP)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();

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
