using Inventory.Extensions;

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