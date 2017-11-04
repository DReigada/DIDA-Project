using System;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class PacManPlayer: AbstractProp {
        public PacManPlayer(int id, Position position) : base(id, position) {
        }
    }
}