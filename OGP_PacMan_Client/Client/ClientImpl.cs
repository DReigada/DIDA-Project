using System;
using System.Collections.Generic;
using System.Linq;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using OGPPacManClient.Interface;

namespace OGPPacManClient.Client {
    internal class ClientImpl : MarshalByRefObject, IPacManClient {
        private readonly BoardController controller;

        public ClientImpl(BoardController controller) {
            this.controller = controller;
            ConnectedClients = new List<ConnectedClient>();
        }

        public List<ConnectedClient> ConnectedClients { get; private set; }

        public void UpdateState(Board board) {
            controller.Update(board);
        }

        //TODO This should be thread safe
        public void UpdateConnectedClients(List<ConnectedClient> clients) {
            var newClients = clients.Where(client => !ConnectedClients.Exists(a => a.Id == client.Id))
                .ToList();
            if (newClients.Count > 0) NewConnectedClients?.BeginInvoke(newClients, null, null);
            ConnectedClients = clients;
        }

        public event Action<List<ConnectedClient>> NewConnectedClients;

        public override object InitializeLifetimeService() {
            return null;
        }
    }
}