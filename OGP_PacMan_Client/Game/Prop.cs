namespace pacman.Game {
    public class Position {
        public int X { get; set; }
        public int Y { get; set; }
    }

    internal abstract class Prop {
        public Position Position { get; set; }
    }
}