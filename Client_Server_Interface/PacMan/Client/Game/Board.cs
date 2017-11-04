using System.Collections.Generic;

namespace ClientServerInterface.PacMan.Client.Game {
    public class Board {
        public Board(List<Ghost> ghosts, List<PacManPlayer> players) {
            Ghosts = ghosts;
            Players = players;
        }

        public List<Ghost> Ghosts { get; }
        public List<PacManPlayer> Players { get; }
        public List<Coin> Coins { get; }
    }
}