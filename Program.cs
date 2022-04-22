using BattleshipBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(policyBuilder =>
{
    policyBuilder.AllowAnyHeader();
    policyBuilder.AllowAnyMethod();
    policyBuilder.SetIsOriginAllowed(_ => true);
});

app.MapHub<BattleshipHub>("/hubs/battleship");

app.Run();