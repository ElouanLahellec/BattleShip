using BattleShip.API;
using Microsoft.OpenApi.Models;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BattleShip API", Version = "v1" });
});
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BattleShip API v1");
    });
}


app.UseCors();  // Activer CORS pour toutes les routes

// Cr�er un point de terminaison pour SignalR
app.MapHub<GameHub>("/play");  // "GameHub" est le hub SignalR que nous allons d�finir

app.Run();
