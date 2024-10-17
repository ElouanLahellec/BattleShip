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

    public int AddCoords(User user, int coordX, int coordY, bool isHit)
    {
        List<int> coords = [coordX, coordY, isHit ? 1 : 0];
        user.plays.Append(coords);
        return user.countHits();
    }

    public List<int> playAI()
    {
        Random random = new Random();
        if (aiDiff == 2)
        {
            List<List<int>> potentialTargets = new List<List<int>>();
            foreach (var play in userB.plays)
            {
                if (play[2] == 1)
                {
                    List<List<int>> neighbors = new List<List<int>>
                    {
                        new List<int> { play[0] - 1, play[1] },
                        new List<int> { play[0] + 1, play[1] },
                        new List<int> { play[0], play[1] - 1 },
                        new List<int> { play[0], play[1] + 1 }
                    };

                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor[0] >= 0 && neighbor[0] < userB.board.grid.Count &&
                            neighbor[1] >= 0 && neighbor[1] < userB.board.grid[0].Count &&
                            !userB.plays.Any(p => p[0] == neighbor[0] && p[1] == neighbor[1]))
                        {
                            potentialTargets.Add(neighbor);
                        }
                    }
                }
            }

            // If we found potential targets, return one of them, otherwise choose randomly
            if (potentialTargets.Count > 0)
            {
                return potentialTargets[random.Next(potentialTargets.Count)];
            }
        }
        
        List<List<int>> coords = new List<List<int>>();
        for (int x = 0; x < userB.board.grid.Count; x++)
        {
            for (int y = 0; y < userB.board.grid[0].Count; y++)
            {
                bool isHit = false;
                for (int i = 0; i < userB.plays.Count; i++)
                {
                    if (x == userB.plays[i][0] && y == userB.plays[i][1])
                    {
                        isHit = true;
                        break;
                    }
                }

                if (!isHit)
                {
                    coords.Add([x, y]);
                }
            }
        }

        if (coords.Count > 0) return coords[random.Next(coords.Count)];
        else return [0, 0];
    }
}