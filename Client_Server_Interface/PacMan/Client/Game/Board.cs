using System;
using System.Collections.Generic;
using System.Linq;

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

        public string PrettyString() {
            var ghosts = string.Join("\n",
                Ghosts.Select(g => $"M, {g.Position.X}, {g.Position.Y}"));

            Func<bool, string> playerPL = a => a ? "P" : "L";

            var players = string.Join("\n",
                Players.Select(p => $"P{p.Id}, {playerPL(p.Alive)}, {p.Position.X}, {p.Position.Y}"));

            var coins = string.Join("\n",
                Coins.Select(c => $"C, {c.Position.X}, {c.Position.Y}"));

            return ghosts + "\n" + players + "\n" + coins;
        }
    }
}