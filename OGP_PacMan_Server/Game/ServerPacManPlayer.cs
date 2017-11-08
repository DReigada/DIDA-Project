using ClientServerInterface.PacMan.Client.Game;

namespace OGP_PacMan_Server.Game {
    public class ServerPacManPlayer : PacManPlayer {
        public ServerPacManPlayer(int id, Position position, int score, bool alive, string name) : base(id, position, score, alive) {
            Name = name;
        }

        public string Name { get; }
    }
}