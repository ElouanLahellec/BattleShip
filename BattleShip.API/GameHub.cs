using BattleShip.API;
using Microsoft.AspNetCore.SignalR;

using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{

    public async Task Join(string room)
    {
        Memory memory = Memory.GetInstance();
        if (!memory.users.ContainsKey(Context.ConnectionId))
        {
            memory.users.Add(Context.ConnectionId, new User(Context.ConnectionId));
        }
        User user = memory.users[Context.ConnectionId];

        Game game = null;
        if (!memory.rooms.ContainsKey(room))
        {
            memory.rooms.Add(room, new Game(room, user));
            game = memory.rooms[room];
        }
        else
        {
            game = memory.rooms[room];
            memory.rooms[room].userB = user;
            
            game.userB.opponent = game.userA;
            game.userA.opponent = game.userB;
            game.playingPlayer = game.userB;
        }
        user.Game = game;
        
        Console.WriteLine($"User {Context.ConnectionId} joined room {room}");
        if (game.isReady())
        {
            Console.WriteLine("Starting game");
            game.state = GameState.PLAYING;
            
            Console.WriteLine($"Sending StartGame to userA with ID: {game.userA.id}");
            Console.WriteLine($"Sending StartGame to userB with ID: {game.userB.id}");
            Board randomBoard = new Board();
            await Clients.Client(game.userA.id).SendAsync("StartGame", randomBoard.PlaceRdmBoats());
            await Clients.Client(game.userB.id).SendAsync("StartGame", randomBoard.PlaceRdmBoats());
            
            await Clients.Client(game.playingPlayer.id).SendAsync("YourTurn");
            Console.WriteLine("Game started");
        }
    }

    public async Task Play(int coordX, int coordY)
    {
        Memory memory = Memory.GetInstance();
        User user = memory.users[Context.ConnectionId];
        await Clients.Client(user.opponent.id).SendAsync("Play", coordX, coordY);
        user.Game.switchPlayingPlayer();
        await Clients.Client(user.Game.playingPlayer.id).SendAsync("YourTurn");
    }
}