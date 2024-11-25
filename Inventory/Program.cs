using Inventory.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Inventory;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureLogging();
        builder.ConfigureServices();
        builder.ConfigureAuthentication();
        builder.ConfigureSwagger();
        builder.ConfigureDatabase();
        builder.ConfigureMediatR();

        var app = builder.Build();

        app.ConfigureMiddleware();
        app.ConfigureEndpoints();

        app.Run();
    }
}