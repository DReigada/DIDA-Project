using ClientServerInterface.PacMan.Client.Game;

namespace OGP_PacMan_Server.Game.PacMan {
    public class Speed {
        public Speed(int x, int y) {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class ServerGhost : Ghost {
        public ServerGhost(GhostColor color, Position pos, int id, Speed speed) : base(color, pos, id) {
            Speed = speed;
        }

        public int Width { get; } = 30;

        public int Length { get; } = 30;

        public Speed Speed { get; }
    }
}