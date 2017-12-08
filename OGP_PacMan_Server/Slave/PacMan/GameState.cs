using System;
using System.Collections.Generic;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;

namespace OGP_PacMan_Server.Slave.PacMan {
    [Serializable]
    public class GameState {
        public GameState(List<Board> boards, List<ConnectedClient> clients, List<Movement> newMovements) {
            Boards = boards;
            Clients = clients;
            NewMovements = newMovements;
        }

        public List<Board> Boards { get; }

        public List<ConnectedClient> Clients { get; }

        public List<Movement> NewMovements { get; }
    }
}