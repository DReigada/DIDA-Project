using System.Collections.Generic;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using OGPPacManClient.Interface;

namespace OGPPacManClient.Client {
    internal class ClientImpl : IPacManClient {
        private readonly BoardController controller;
        private List<ConnectedClient> connectedClients;

        public ClientImpl(BoardController controller) {
            this.controller = controller;
        }

        public void UpdateState(Board board) {
            controller.Update(board);
        }

        public void UpdateConnectedClients(List<ConnectedClient> clients) {
            connectedClients = clients;
        }
    }
}