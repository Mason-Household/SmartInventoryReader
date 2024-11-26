using System.Diagnostics.CodeAnalysis;
using Inventory.Extensions;

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

        var app = builder.Build();

        app.ConfigureMiddleware();
        app.ConfigureEndpoints();

        app.Run();
    }
}