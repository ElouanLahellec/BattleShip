namespace BattleShip.API
{
    public class User
    {
        private string id {  get; set; }
        private string gameId { get; set; }
        private string opponent {  get; set; }

        public User(string id, string gameId, string opponent)
        {
            this.id = id;
            this.gameId = gameId;
            this.opponent = opponent;
        }

        public void SetOpponent(string opponent)
        {
            this.opponent = opponent;
        }

        public string GetOpponent()
        {
            return opponent;
        }
    }
}
