namespace ClientServerInterface.PacMan.Client.Game {
    public class Ghost : AbstractProp {
        public Ghost(GhostColor color, Position pos, int id) : base(id, pos) {
            Color = color;
        }

        public GhostColor Color { get; }
    }

    public enum GhostColor {
        Red,
        Pink,
        Yellow
    }
}