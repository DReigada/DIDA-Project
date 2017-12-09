using System;
using System.Collections.Generic;
using System.Linq;
using OGP_PacMan_Server.Game.PacMan;

namespace ClientServerInterface.PacMan.Client.Game {
    [Serializable]
    public class Board {
        public Board(int roundId, List<ServerGhost> ghosts, List<PacManPlayer> players, List<Coin> coins) {
            Ghosts = ghosts;
            Players = players;
            Coins = coins;
            RoundID = roundId;
        }

        public List<ServerGhost> Ghosts { get; }
        public List<PacManPlayer> Players { get; }
        public List<Coin> Coins { get; }

        public int RoundID { get; }

        public string PrettyString() {
            var ghosts = string.Join(Environment.NewLine,
                Ghosts.Select(g => $"M, {g.Position.X}, {g.Position.Y}"));

            Func<bool, string> playerPL = a => a ? "P" : "L";

            var players = string.Join(Environment.NewLine,
                Players.Select(p => $"P{p.Id}, {playerPL(p.Alive)}, {p.Position.X}, {p.Position.Y}"));

            var coins = string.Join(Environment.NewLine,
                Coins.Select(c => $"C, {c.Position.X}, {c.Position.Y}"));

            return ghosts + Environment.NewLine + players + Environment.NewLine + coins + Environment.NewLine;
        }
    }
}