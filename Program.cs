using BattleshipBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ExampleHub>("/hubs/example");

app.Run();