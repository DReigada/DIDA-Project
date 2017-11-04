using System.Collections.Specialized;
using ClientServerInterface.PacMan.Client.Game;

namespace OGP_PacMan_Server.Game {
    public class ServerPacManPlayer : PacManPlayer {
        public ServerPacManPlayer(int id, Position position, string name) : base(id, position) {
            Name = name;
        }

        public string Name { get; }
    }
}