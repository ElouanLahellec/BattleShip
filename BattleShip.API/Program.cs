using BattleShip.API;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders(); // Optional: Clears default providers
builder.Logging.AddConsole(); // Add console logging
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Set minimum log level

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();  // Allow all domains (useful for local development)
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors();  // Enable CORS for all routes

// Create a SignalR endpoint
app.MapHub<GameHub>("/play");

app.Run();