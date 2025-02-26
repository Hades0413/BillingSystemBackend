using BillingSystemBackend.Data;
using BillingSystemBackend.Models;
using BillingSystemBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Kestrel
builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(8080); });

// Configuración de CORS
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

builder.Services.AddDbContext<TipoComprobanteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));

builder.Services.AddDbContext<ClienteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sql")));

// Registro de servicios
builder.Services.AddScoped<UsuarioDbContext>();
builder.Services.AddScoped<RubroDbContext>();
builder.Services.AddScoped<EmpresaDbContext>();
builder.Services.AddScoped<UnidadDbContext>();
builder.Services.AddScoped<CategoriaDbContext>();
builder.Services.AddScoped<ProductoDbContext>();
builder.Services.AddScoped<TipoComprobanteDbContext>();
builder.Services.AddScoped<ClienteDbContext>();

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RubroService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<UnidadService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<TipoComprobanteService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<TipoClienteService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? string.Empty)) 
        };
    });

builder.Services.AddAuthorization(options =>
{
    
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseCors("AllowAngularApp");

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    "default",
    "{controller=Usuario}/{action=Registrar}/{id?}");

app.Run();
