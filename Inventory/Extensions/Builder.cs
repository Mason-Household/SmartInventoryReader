using MediatR;
using Serilog;
using FirebaseAdmin;
using Inventory.Data;
using FluentValidation;
using System.Reflection;
using FirebaseAdmin.Auth;
using Inventory.Services;
using Inventory.Middleware;
using Inventory.Properties;
using Inventory.Repositories;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
        // Initialize Firebase Admin SDK
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"))
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy
                .WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
        });

        builder.Services.AddSingleton(FirebaseAuth.DefaultInstance);
        builder.Services.AddControllers();
        builder.Services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Add Organization Service
        builder.Services.AddScoped<IOrganizationService, OrganizationService>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    }

    public static void ConfigureMediatR(this WebApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        builder.Services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequest<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        builder.Services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        builder.Services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(AbstractValidator<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://securetoken.google.com/" + builder.Configuration["Firebase:ProjectId"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://securetoken.google.com/" + builder.Configuration["Firebase:ProjectId"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Firebase:ProjectId"],
                    ValidateLifetime = true
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

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(defaultConnectionString))
        {
            throw new InvalidOperationException("Default connection string is not configured.");
        }

        // Register InventoryDbContext
        builder.Services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(
                defaultConnectionString,
                x => x.MigrationsAssembly(ConfigurationConstants.MigrationsAssembly)
            ));

        // Register AppDbContext
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                defaultConnectionString,
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
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<TenantMiddleware>();

        app.MapControllers();
        app.MapHealthChecks(ConfigurationConstants.HealthCheck);
    }
}