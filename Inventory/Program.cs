using Inventory.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.ConfigureMediatR();
builder.ConfigureSwagger();
builder.ConfigureServices();
builder.ConfigureDatabase();
builder.ConfigureAuthentication();

var app = builder.Build();
app.ConfigureMiddleware();
app.Run();