using Microsoft.EntityFrameworkCore;
using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using BillingSystemBackend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); 
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
            .WithHeaders("Content-Type", "Authorization");
    });
});

builder.Services.AddDbContext<UsuarioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));

builder.Services.AddDbContext<RubroDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));

builder.Services.AddDbContext<EmpresaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));

builder.Services.AddDbContext<UnidadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));

builder.Services.AddDbContext<CategoriaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));

builder.Services.AddDbContext<ProductoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));


builder.Services.AddScoped<UsuarioDbContext>();
builder.Services.AddScoped<RubroDbContext>();
builder.Services.AddScoped<EmpresaDbContext>();
builder.Services.AddScoped<UnidadDbContext>();
builder.Services.AddScoped<CategoriaDbContext>();
builder.Services.AddScoped<ProductoDbContext>();

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RubroService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<UnidadService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ProductoService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Habilitar CORS
app.UseCors("AllowAngularApp");

// Configuración de enrutamiento y autorización
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Configuración de las rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Registrar}/{id?}");

app.Run();