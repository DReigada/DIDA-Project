using System;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Ghost : AbstractProp {
        public Ghost(GhostColor color, Position pos, int id) : base(id, pos) {
            Color = color;
        }

        public GhostColor Color { get; }
    }

    [Serializable]
    public enum GhostColor {
        Red,
        Pink,
        Yellow
    }
}