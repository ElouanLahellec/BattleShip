namespace BattleShip.API;

public enum GameState
{
    WAITING, PLAYING, RESULT
}

public class Game
{
    public String id;
    public GameState state = GameState.WAITING;
    
    public User userA {  get; set; }
    public User userB {  get; set; }
    public User playingPlayer {  get; set; }
    public bool aiMode { get; set; }
    public int aiDiff { get; set; }

    public Game(String id, User userA)
    {
        this.id = id;
        this.userA = userA;
        playingPlayer = userA;
        aiMode = false;
        aiDiff = 0;
    }

    public bool isReady()
    {
        return state == GameState.WAITING && userA != null && userB != null;
    }

    public void switchPlayingPlayer()
    {
        if (playingPlayer == userA) playingPlayer = userB; else playingPlayer = userA;
    }

    public List<int> playAI()
    {
        Random random = new Random();
        return [random.Next(userA.board.grid.Count), random.Next(userA.board.grid[0].Count)];
    }
}