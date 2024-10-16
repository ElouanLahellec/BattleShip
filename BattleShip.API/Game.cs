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

    public Game(String id, User userA)
    {
        this.id = id;
        this.userA = userA;
        this.playingPlayer = userA;
    }

    public bool isReady()
    {
        return state == GameState.WAITING && userA != null && userB != null;
    }

    public void switchPlayingPlayer()
    {
        if (playingPlayer == userA)
        {
            playingPlayer = userB;
        }
        else
        {
            playingPlayer = userA;
        }
    }
}