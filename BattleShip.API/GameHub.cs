using BattleShip.API;
using Microsoft.AspNetCore.SignalR;

using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{

    public async Task Rewind(string gameid)
    {
        Console.WriteLine($"Asked to rewind game {gameid}");
        Memory memory = Memory.GetInstance();
        if (memory.rooms.ContainsKey(gameid))
        {
            Console.WriteLine($"found game {gameid}");
            Game game = memory.rooms[gameid];
            if (game.state == GameState.RESULT)
            {
                Console.WriteLine($"Sending data {gameid}");
                await Clients.Client(Context.ConnectionId).SendAsync("Rewind", game.userA.board.grid, game.userB.board.grid, game.userA.plays, game.userB.plays, game.gridRows, game.gridCols);
            }
        }
    }
    
    public async Task Join(string room, int  gridRows, int gridCols)
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
            memory.rooms.Add(room, new Game(room, user, gridRows, gridCols));
            game = memory.rooms[room];
            game.gridRows = gridRows;
            game.gridCols = gridCols;
        }
        else
        {
            game = memory.rooms[room];
            if (game.state != GameState.WAITING) return;
            game.userB = user;
            game.playingPlayer = game.userB;
        }
        user.Game = game;
        
        Console.WriteLine($"User {Context.ConnectionId} joined room {room}");
        await startGame(game);
    }

    public async Task AskAIToJoin(string aiMode)
    {
        if (aiMode == "1" || aiMode == "2" || aiMode == "3")
        {
            Memory memory = Memory.GetInstance();
            User user = memory.users[Context.ConnectionId];
            Game game = user.Game;
            game.userB = new User();
            game.userB.Game = game;
            game.aiMode = true;
            game.aiDiff = Int32.Parse(aiMode);

            startGame(game);
            
            game.playAI();
        }
    }

    private async Task startGame(Game game)
    {
        
        if (game.isReady())
        {
            Console.WriteLine("Starting game");
            game.userB.opponent = game.userA;
            game.userA.opponent = game.userB;
            game.state = GameState.PLAYING;
            
            Console.WriteLine($"Sending StartGame to userA with ID: {game.userA.id}");
            Console.WriteLine($"Sending StartGame to userB with ID: {game.userB.id}");

            game.initBoards();
            await Clients.Client(game.userA.id).SendAsync("StartGame", game.userA.board.grid, game.gridRows, game.gridCols);
            if (!game.aiMode)
                await Clients.Client(game.userB.id).SendAsync("StartGame", game.userB.board.grid, game.gridRows, game.gridCols);
            
            await Clients.Client(game.playingPlayer.id).SendAsync("YourTurn");
            Console.WriteLine("Game started");
        }
    }

    public async Task<char> Play(int coordX, int coordY)
    {
        Memory memory = Memory.GetInstance();
        User user = memory.users[Context.ConnectionId];
        Game game = user.Game;
        if (game.state == GameState.PLAYING)
        {
            if (!user.Equals(game.playingPlayer))
                throw new InvalidOperationException("It's not your turn!");
            bool result = user.opponent.board.IsHit(coordX, coordY);
            user.plays.Add([coordX, coordY, result? 1 : 0]);
            if (game.AddCoords(user, coordX, coordY, result) >= 15)
            { 
                game.state = GameState.RESULT;
            }
            if (game.aiMode)
            {
                if (game.state == GameState.PLAYING)
                {
                    List<int> coords = game.playAI();
                    bool aiResult = user.board.IsHit(coords[0], coords[1]);
                    game.userB.plays.Add([coords[0], coords[1], aiResult ? 1 : 0]);
                    await Clients.Client(user.id).SendAsync("Play", coords[0], coords[1]);

                    if (game.AddCoords(game.userB, coords[0], coords[1], aiResult) >= 15)
                    {
                        game.state = GameState.RESULT;
                    }
                }
            }
            else
            {
                await Clients.Client(user.opponent.id).SendAsync("Play", coordX, coordY);
                game.switchPlayingPlayer();
            }
            if (game.state == GameState.PLAYING)
                await Clients.Client(game.playingPlayer.id).SendAsync("YourTurn");
            return result ? 'O' : 'X';
        }
        else
        {
            throw new InvalidOperationException("The game is not playing");
        }
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