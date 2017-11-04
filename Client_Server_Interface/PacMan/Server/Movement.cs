using System;

namespace ClientServerInterface.PacMan.Server {
    [Serializable]
    public class Movement {
        public enum Direction {
            Up,
            Down,
            Left,
            Right
        }

        public Movement(int id, Direction direction) {
            Id = id;
            Direct = direction;
        }

        public int Id { get; set; }

        public Direction Direct { get; }
    }
}