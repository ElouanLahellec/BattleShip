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
            if (game.state != GameState.WAITING) return;
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
            await Clients.Client(game.userA.id).SendAsync("StartGame", game.userA.board.PlaceRdmBoats());
            await Clients.Client(game.userB.id).SendAsync("StartGame", game.userB.board.PlaceRdmBoats());
            
            await Clients.Client(game.playingPlayer.id).SendAsync("YourTurn");
            Console.WriteLine("Game started");
        }
    }

    public async Task AskAIToJoin(string aiMode)
    {
        if (aiMode == "1" || aiMode == "2" || aiMode == "3")
        {
            Memory memory = Memory.GetInstance();
            User user = memory.users[Context.ConnectionId];
            Game game = user.Game;
            game.aiMode = true;
            game.aiDiff = Int32.Parse(aiMode);
            
            await Clients.Client(game.userA.id).SendAsync("StartGame", game.userA.board.PlaceRdmBoats());
            game.playAI();
            await Clients.Client(game.userA.id).SendAsync("YourTurn");
        }
    }

    public async Task<char> Play(int coordX, int coordY)
    {
        Memory memory = Memory.GetInstance();
        User user = memory.users[Context.ConnectionId];
        if (user.Game.aiMode)
        {
            List<int> coords = user.Game.playAI();
            await Clients.Client(user.id).SendAsync("Play", coords[0], coords[1]);
        }
        else
        {
            await Clients.Client(user.opponent.id).SendAsync("Play", coordX, coordY);
            user.Game.switchPlayingPlayer();
        }

        await Clients.Client(user.Game.playingPlayer.id).SendAsync("YourTurn");
        return 'X';
    }

    public async Task CreateNewGame()
    {
        Memory memory = Memory.GetInstance();
        User user = memory.users[Context.ConnectionId];

        string gameid = GenerateRandomString();
        
        await Clients.Client(user.Game.userA.id).SendAsync("OpenNewGame", gameid);
        await Clients.Client(user.Game.userB.id).SendAsync("OpenNewGame", gameid);
    }
    
    
    private static string GenerateRandomString(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}