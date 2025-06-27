using AutoMapper;
using Uttt.Micro.Cupon;
using Uttt.Micro.Cupon.Data;
using Uttt.Micro.Cupon.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// HttpContextAccessor for accessing HTTP context
builder.Services.AddHttpContextAccessor();

// Database configuration con política de reintentos
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    options.UseSqlServer(
        config.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    );
});

// AutoMapper configuration
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);

// Add logging
builder.Services.AddLogging();

// API documentation
builder.Services.AddEndpointsApiExplorer();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger configuration with JWT support
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Microservicio Cupones",
        Version = "v1",
        Description = "API para gestión de cupones de descuento"
    });
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: Bearer Generated-JWT-Token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[] {}
        }
    });
});

// Authentication and Authorization
builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Aplicar migraciones automáticas al inicio con manejo de errores
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Verificando migraciones pendientes para Cupones...");
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Esperar a que SQL Server esté disponible (especialmente en Docker)
        if (!dbContext.Database.CanConnect())
        {
            logger.LogWarning("No se puede conectar a la base de datos. Reintentando en 5 segundos...");
            Thread.Sleep(5000);
        }

        // Verificar migraciones pendientes
        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Aplicando {count} migraciones pendientes...", pendingMigrations.Count);
            dbContext.Database.Migrate();
            logger.LogInformation("Migraciones aplicadas exitosamente");
        }
        else
        {
            logger.LogInformation("No hay migraciones pendientes");
        }

        // Verificación final de conexión
        if (dbContext.Database.CanConnect())
        {
            logger.LogInformation("Conexión con SQL Server establecida correctamente");
        }
        else
        {
            logger.LogError("Error crítico: No se pudo conectar a SQL Server después de migraciones");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fatal durante la aplicación de migraciones");
        throw; // Detener la aplicación si hay errores críticos
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cupones API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();