using System;
using System.Collections.Generic;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Board {
        public Board(List<Ghost> ghosts, List<PacManPlayer> players, List<Coin> coins) {
            Ghosts = ghosts;
            Players = players;
            Coins = coins;
        }

        public List<Ghost> Ghosts { get; }
        public List<PacManPlayer> Players { get; }
        public List<Coin> Coins { get; }

    }
}