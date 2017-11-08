using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;

namespace OGP_PacMan_Server.Game {
    public class ServerPacManPlayer : PacManPlayer {
        public ServerPacManPlayer(int id, Position position, Movement.Direction direction, string name) :
            base(id, position, direction) {
            Name = name;
        }

        public string Name { get; }
    }
}