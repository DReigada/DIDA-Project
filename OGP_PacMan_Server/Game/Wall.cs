using ClientServerInterface.PacMan.Client.Game;

namespace OGP_PacMan_Server.Game {
    public class Wall : AbstractProp {
        public Wall(int id, Position pos) : base(id, pos) {
        }

        public int Width { get; } = 15;
        public int Length { get; } = 95;
    }
}