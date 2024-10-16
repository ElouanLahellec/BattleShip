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
    public List<List<int>> playsUserA { get; set; }
    public List<List<int>> playsUserB { get; set; }
    public int hitsUserA {  get; set; }
    public int hitsUserB { get; set; }


    public Game(String id, User userA)
    {
        this.id = id;
        this.userA = userA;
        this.playingPlayer = userA;
        playsUserA = new List<List<int>>();
        playsUserB = new List<List<int>>();
        int hitsUserA = 0;
        int hitsUserB = 0;
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

    public int AddCoords(User user, int coordX, int coordY, bool isHit)
    {
        List<int> coords = new List<int>();
        coords.Add(coordX);
        coords.Add(coordY);
        int count = 0;
        if (userA.Equals(user))
        {
            playsUserA.Add(coords);
            if (isHit)
                hitsUserA++;
            count = hitsUserA;
        }
        else if (userB.Equals(user))
        {
            playsUserB.Add(coords);
            if (isHit)
                hitsUserB++;
            count = hitsUserB;
        }
        return count;
    }
}