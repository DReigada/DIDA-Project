using System;
using System.Collections.Generic;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;

namespace OGP_PacMan_Server.ISlave.PacMan {
    [Serializable]
    public class GameState {
        public GameState(Board board, List<IPacManClient> pacManClients, List<Movement> newMovements) {
            Board = board;
            PacManClients = pacManClients;
            NewMovements = newMovements;
        }

        public Board Board { get; }

        public List<IPacManClient> PacManClients { get; }

        public List<Movement> NewMovements { get; }
    }
}