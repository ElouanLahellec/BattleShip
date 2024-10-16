namespace BattleShip.API;

public class Memory
{
    private static Memory instance;

    public static Memory GetInstance()
    {
        if (instance == null)
        {
            instance = new Memory();
        }
        return instance;
    }
    
    
    public Dictionary<string, User> users { get; }
    public Dictionary<string, Game> rooms { get; }

    private Memory()
    {
        Console.WriteLine("Initializing Memory");
        users = new();
        rooms = new();
    }
    
    
}