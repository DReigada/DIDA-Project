using System;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Position {
        public Position(int x, int y) {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    [Serializable]
    public abstract class AbstractProp {
        protected AbstractProp(int id, Position position) {
            Id = id;
            Position = position;
        }

        public int Id { get; }
        public Position Position { get; }
    }
}