using Microsoft.OpenApi.Models;
using Serilog;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Security.Claims;
using Inventory.Properties;
using Inventory.Data;  // We'll create this
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Inventory.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(ConfigurationConstants.SwaggerConfig.Version, 
        new OpenApiInfo 
        { 
            Title = ConfigurationConstants.SwaggerConfig.Title, 
            Version = ConfigurationConstants.SwaggerConfig.Version 
        }
    );
    
    // Configure JWT Authentication in Swagger
    c.AddSecurityDefinition(ConfigurationConstants.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = ConfigurationConstants.AuthenticationScheme.ToLower(),
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = ConfigurationConstants.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(ConfigurationConstants.AuthenticationScheme)
    .AddBearerToken(options =>
    {
        options.Events = new BearerTokenEvents
        {
            OnMessageReceived = context =>
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, "John Doe"),
                    new(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, ConfigurationConstants.AuthenticationScheme);
                context.Principal = new ClaimsPrincipal(identity);
                context.Success();
                return Task.CompletedTask;
            }
        };
    });

// Add Health Checks
builder.Services.AddHealthChecks();


// Replace MongoDB with PostgreSQL/EF Core
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("Inventory")
    ));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks(ConfigurationConstants.HealthCheck);

app.Run();
