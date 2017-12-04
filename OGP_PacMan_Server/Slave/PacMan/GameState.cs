using System;
using System.Collections.Generic;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;

namespace OGP_PacMan_Server.Slave.PacMan {
    [Serializable]
    public class GameState {
        public GameState(Board board, List<ConnectedClient> clients, List<Movement> newMovements) {
            Board = board;
            Clients = clients;
            NewMovements = newMovements;
        }

        public Board Board { get; }

        public List<ConnectedClient> Clients { get; }

        public List<Movement> NewMovements { get; }
    }
}