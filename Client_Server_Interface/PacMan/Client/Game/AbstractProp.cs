namespace ClientServerInterface.PacMan.Client.Game {
    public class Position {
        public Position(int x, int y) {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public abstract class AbstractProp {
        public AbstractProp(int id, Position position) {
            Id = id;
            Position = position;
        }

        public int Id { get; protected set; }
        public Position Position { get; set; }
    }
}