using System;
using ClientServerInterface.PacMan.Server;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class PacManPlayer : AbstractProp {
        public PacManPlayer(int id, Position position, Movement.Direction direction, int score, bool alive) : base(id,
            position) {
            Score = score;
            Alive = alive;
            Direction = direction;
        }

        public Movement.Direction Direction { get; set; }

        public int Score { get; set; }

        public bool Alive { get; set; }

        public PacManPlayer Copy() {
            return new PacManPlayer(Id, Position.Copy(), Direction, Score, Alive);
        }
    }
}