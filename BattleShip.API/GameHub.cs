using BattleShip.API;
using Microsoft.AspNetCore.SignalR;

using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    private Dictionary<string, User> users = new();
    
    public GameHub(ILogger<GameHub> logger)
    {
        Console.WriteLine("Initializing GameHub");
    }

    public async Task Join(string room)
    {
        Console.WriteLine($"User {Context.ConnectionId} joined room {room}");
        if (users.Count != 0)
        {
            users.Add(Context.ConnectionId, new User(Context.ConnectionId, room, users.First().Key));
            users.First().Value.SetOpponent(Context.ConnectionId);
            Board randomBoard = new Board();
            await Clients.Client(users.First().Key).SendAsync("StartGame", randomBoard.PlaceRdmBoats());
            await Clients.Client(Context.ConnectionId).SendAsync("StartGame", randomBoard.PlaceRdmBoats());
        }
        else
        {
            users.Add(Context.ConnectionId, new User(Context.ConnectionId, room, String.Empty));
        }
    }

    public async Task Play(int coordX, int coordY)
    {
        if (users.TryGetValue(Context.ConnectionId, out User user))
        {
            await Clients.Client(user.GetOpponent()).SendAsync("Play", coordX, coordY);
            await Clients.Client(Context.ConnectionId).SendAsync("YourTurn");
        }
    }
}