using System;
using ClientServerInterface.PacMan.Server;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class PacManPlayer : AbstractProp {
        public PacManPlayer(int id, Position position, Movement.Direction direction) : base(id, position) {
            this.direction = direction;
        }

        public Movement.Direction direction { get; }
    }
}