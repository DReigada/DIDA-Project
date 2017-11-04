namespace ClientServerInterface.PacMan.Server {
    public class Movement {

        public Movement(int id, Direction direction) {
            Id = id; 
            Direct = direction;
        }

        public int Id { get; set; }

        public Direction Direct { get; }

        public enum Direction {
            Up,
            Down,
            Left,
            Right
        }
    }
}