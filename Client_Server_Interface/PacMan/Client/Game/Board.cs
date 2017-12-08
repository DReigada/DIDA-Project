using System;
using System.Collections.Generic;
using OGP_PacMan_Server.Game.PacMan;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Board {
        public Board(int roundID, List<ServerGhost> ghosts, List<PacManPlayer> players, List<Coin> coins) {
            RoundID = roundID;
            Ghosts = ghosts;
            Players = players;
            Coins = coins;
        }

        public int  RoundID { get; set; }
        public List<ServerGhost> Ghosts { get; }
        public List<PacManPlayer> Players { get; }
        public List<Coin> Coins { get; }
    }
}