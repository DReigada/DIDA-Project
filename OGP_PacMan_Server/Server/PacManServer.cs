using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Game;

namespace OGP_PacMan_Server.Server {
    public class PacManServer : MarshalByRefObject, IPacmanServer {
        private int gameSpeed;

        private int numberPlayers;

        private List<ClientInfo> clients;

        private PacManGame game;

        private List<IPacManClient> pacManClients;

        private System.Timers.Timer gameTimer;

        public PacManServer(int gameSpeed, int numberPlayers) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            clients = new List<ClientInfo>();
            pacManClients = new List<IPacManClient>();
            game = new PacManGame(numberPlayers);

            gameTimer = new System.Timers.Timer();
            gameTimer.Elapsed += new ElapsedEventHandler(TimeEvent);
            gameTimer.Interval = gameSpeed;
            gameTimer.Enabled = true;

        }

        private void TimeEvent(object source, ElapsedEventArgs e) {
            game.NextState();
            if (game.GameEnded) {
                gameTimer.Enabled = false;
            }
            UpdateState();
        }

        public GameProps RegisterClient(ClientInfo client) {
            clients.Add(client);

            IPacManClient pacManClient = (IPacManClient) Activator.GetObject(typeof(IPacManClient), client.Url);
            pacManClients.Add(pacManClient);

            var props = new GameProps(gameSpeed, numberPlayers, clients.Count);
            if (clients.Count == numberPlayers){
                game.Start(clients);
                ThreadStart theardStart = UpdateState;
                Thread thread = new Thread(theardStart);
                thread.Start();
                UpdateState();
            }
            return props;
        }

        public void SendAction(Movement movement) {
            game.AddMovements(movement);
        }

        public void UpdateState() {
            Board board = game.Board;
            foreach (IPacManClient pacManClient in pacManClients)
                pacManClient.UpdateState(board);
        }

        public void UpdateConnectedClients() {
            //TODO: Implementar ConnectedClients
        }
    }
}