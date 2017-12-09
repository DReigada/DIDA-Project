using System;
using System.Collections.Generic;
using System.Linq;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using OGPPacManClient.Interface;
using OGPPacManClient.PuppetMaster;

namespace OGPPacManClient.Client {
    internal class ClientImpl : MarshalByRefObject, IPacManClient {
        private readonly IDictionary<int, Board> boards;
        private readonly BoardController controller;

        private readonly Action<string> updateServer;

        public ClientImpl(BoardController controller, Action<string> updateServer) {
            this.controller = controller;
            ConnectedClients = new List<ConnectedClient>();
            this.updateServer = updateServer;
            boards = new Dictionary<int, Board>();
        }

        public List<ConnectedClient> ConnectedClients { get; private set; }

        public void UpdateState(Board board) {
            ClientPuppet.Instance.Wait();
            boards.Add(board.RoundID, board);
            controller.Update(board);
        }

        public void UpdateConnectedClients(List<ConnectedClient> clients) {
            ClientPuppet.Instance.Wait();
            lock (ConnectedClients) {
                var newClients = clients.Where(client => !ConnectedClients.Exists(a => a.Id == client.Id))
                    .ToList();
                if (newClients.Count > 0) NewConnectedClients?.BeginInvoke(newClients, null, null);
                ConnectedClients = clients;
            }
        }

        public void UpdateServer(ServerInfo serverInfo) {
            ClientPuppet.Instance.Wait();
            updateServer(serverInfo.Url);
        }

        public Board GetRoundBoard(int roundId) {
            return boards[roundId];
        }

        public event Action<List<ConnectedClient>> NewConnectedClients;

        public override object InitializeLifetimeService() {
            return null;
        }
    }
}