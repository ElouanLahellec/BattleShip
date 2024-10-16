using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public async Task Join(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).SendAsync("StartGame");
    }

    public async Task Play(string gameId, int row, int col)
    {
        await Clients.Group(gameId).SendAsync("Play", row, col);
    }
}
