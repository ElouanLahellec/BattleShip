using BattleShip.API;

var builder = WebApplication.CreateBuilder(args);

// Ajouter le support CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader()
               .AllowAnyMethod()
               .AllowAnyOrigin();  // Autoriser tous les domaines (c'est pratique pour du d�veloppement local)
    });
});

var app = builder.Build();

app.UseCors();  // Activer CORS pour toutes les routes

// Cr�er un point de terminaison pour SignalR
app.MapHub<GameHub>("/play");  // "GameHub" est le hub SignalR que nous allons d�finir

app.Run();
