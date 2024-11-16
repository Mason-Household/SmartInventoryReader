using Serilog;
using Inventory.Data;
using FluentValidation;
using Inventory.Properties;
using System.Security.Claims;
using Inventory.Repositories;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.BearerToken;

namespace Inventory.Extensions;

public static class BuilderExtensions
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(ConfigurationConstants.FullLogFilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }

    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(ConfigurationConstants.AuthenticationScheme)
            .AddBearerToken(options =>
            {
                options.Events = new BearerTokenEvents
                {
                    OnMessageReceived = context =>
                    {
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, ConfigurationConstants.JwtConfig.Claims.UserId),
                            new(ClaimTypes.Role, ConfigurationConstants.JwtConfig.Claims.Role)
                        };
                        var identity = new ClaimsIdentity(claims, ConfigurationConstants.AuthenticationScheme);
                        context.Principal = new ClaimsPrincipal(identity);
                        context.Success();
                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(ConfigurationConstants.SwaggerConfig.Version,
                new OpenApiInfo
                {
                    Title = ConfigurationConstants.SwaggerConfig.Title,
                    Version = ConfigurationConstants.SwaggerConfig.Version
                }
            );

            c.AddSecurityDefinition(ConfigurationConstants.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = ConfigurationConstants.JwtConfig.Description,
                Name = ConfigurationConstants.JwtConfig.SecurityScheme,
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
    }

    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString(ConfigurationConstants.DefaultConnection),
                x => x.MigrationsAssembly(ConfigurationConstants.MigrationsAssembly)
            ));
    }

    public static void ConfigureMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static void ConfigureEndpoints(this WebApplication app)
    {
        app.MapControllers();
        app.MapHealthChecks(ConfigurationConstants.HealthCheck);
    }
}