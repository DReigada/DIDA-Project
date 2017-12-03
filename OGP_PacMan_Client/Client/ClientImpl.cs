using System;
using System.Collections.Generic;
using System.Linq;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using OGPPacManClient.Interface;
//using OGPPacManClient.PuppetMaster;

namespace OGPPacManClient.Client {
    internal class ClientImpl : MarshalByRefObject, IPacManClient {
        private readonly BoardController controller;

        public delegate void ChangeServerDel(string url);

        public ChangeServerDel handler;

        public ClientImpl(BoardController controller, ChangeServerDel hanlder) {
            this.controller = controller;
            ConnectedClients = new List<ConnectedClient>();
            this.handler = hanlder;
        }

        public List<ConnectedClient> ConnectedClients { get; private set; }

        public void UpdateState(Board board) {
            //ClientPuppet.Wait();
            controller.Update(board);
        }

        //TODO This should be thread safe
        public void UpdateConnectedClients(List<ConnectedClient> clients) {
            //ClientPuppet.Wait();
            var newClients = clients.Where(client => !ConnectedClients.Exists(a => a.Id == client.Id))
                .ToList();
            if (newClients.Count > 0) NewConnectedClients?.BeginInvoke(newClients, null, null);
            ConnectedClients = clients;
        }

        public void UpdateServer(ServerInfo serverInfo) {
            handler(serverInfo.Url);
        }

        public event Action<List<ConnectedClient>> NewConnectedClients;

        public override object InitializeLifetimeService() {
            return null;
        }
    }
}