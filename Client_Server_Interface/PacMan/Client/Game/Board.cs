using System;
using System.Collections.Generic;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Board {
        public Board(int roundID, List<Ghost> ghosts, List<PacManPlayer> players, List<Coin> coins) {
            RoundID = roundID;
            Ghosts = ghosts;
            Players = players;
            Coins = coins;
        }

        public int  RoundID { get; set; }
        public List<Ghost> Ghosts { get; }
        public List<PacManPlayer> Players { get; }
        public List<Coin> Coins { get; }
    }
}