using Inventory.Data;
using Inventory.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.ConfigureMediatR();
builder.ConfigureSwagger();
builder.ConfigureServices();
builder.ConfigureDatabase();
builder.ConfigureAuthentication();

var app = builder.Build();
app.MigrateDatabase<InventoryDbContext>();
app.MigrateDatabase<AppDbContext>();
app.ConfigureMiddleware();
app.Run();