using BattleShip.API;

namespace BattleShip.API;
    
    
public class User
{
    public string id {  get; }
    public Game Game { get; set; }
    public User opponent {  get; set; }
    public Board board { get; set; }
    public List<List<int>> plays = new ();

    public User()
    {
        board = new Board();
    }
    
    public User(string id)
    {
        this.id = id;
        board = new Board();
    }

    public int countHits()
    {
        int counter = 0;
        for (int i = 0; i < plays.Count; i++)
        {
            if (plays[i][2] == 1) counter++;
        }
        
        return counter;
    }
}
