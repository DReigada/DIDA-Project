using System;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class PacManPlayer: AbstractProp {
        public PacManPlayer(int id, Position position, int score, bool alive) : base(id, position) {
            Score = score;
            Alive = alive;
        }
        public int Score { get; set; }

        public bool Alive { get; set; }
    }
}