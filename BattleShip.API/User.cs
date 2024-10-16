using BattleShip.API;

namespace BattleShip.API;
    
    
public class User
{
    public string id {  get; }
    public Game Game { get; set; }
    public User opponent {  get; set; }
    public Board board { get; set; }

    public User()
    {
        board = new Board();
    }
    
    public User(string id)
    {
        this.id = id;
        board = new Board();
    }
    
    
}
