namespace BattleshipBackend
{
    public class GameRoom
    {
        string player1, player2;
        string roomName;
        public GameRoom(string player1, string player2, string roomName)
        {
            this.player1 = player1;
            this.player2 = player2;
            this.roomName = roomName;
        }
    }
}
