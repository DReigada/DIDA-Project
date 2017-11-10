using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Game.PacMan;
using OGP_PacMan_Server.PuppetMaster;

namespace OGP_PacMan_Server.Server {
    public class PacManServer : MarshalByRefObject, IPacmanServer {
        private int gameSpeed;

        private int numberPlayers;

        private List<ConnectedClient> clients;

        private PacManGame game;

        private List<IPacManClient> pacManClients;

        private System.Timers.Timer gameTimer;

        public PacManServer(int gameSpeed, int numberPlayers) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            clients = new List<ConnectedClient>();
            pacManClients = new List<IPacManClient>();
            game = new PacManGame(numberPlayers);

            gameTimer = new System.Timers.Timer();
            gameTimer.Elapsed += new ElapsedEventHandler(TimeEvent);
            gameTimer.Interval = gameSpeed;

        }

        private void TimeEvent(object source, ElapsedEventArgs e) {
            game.NextState();
            if (game.GameEnded) {
                gameTimer.Enabled = false;
                Console.WriteLine("GAME OVER!!!!");
            }
            UpdateState();
        }

        public GameProps RegisterClient(ClientInfo client) {
            ServerPuppet.Wait();
            Console.WriteLine(client.Url);
            clients.Add(new ConnectedClient(clients.Count + 1, client.Url));

            IPacManClient pacManClient = (IPacManClient) Activator.GetObject(typeof(IPacManClient), client.Url + "/PacManClient");
            pacManClients.Add(pacManClient);

            ThreadStart updateClient = UpdateConnectedClients;
            Thread updateThread = new Thread(updateClient);
            updateThread.Start();

            GameProps props = new GameProps(gameSpeed, numberPlayers, clients.Count);
            if (clients.Count == numberPlayers){
                game.Start(clients);
                ThreadStart theardStart = UpdateState;
                Thread thread = new Thread(theardStart);
                thread.Start();
                UpdateState();
                gameTimer.Enabled = true;
            }
            return props;
        }

        public void SendAction(Movement movement) {
            ServerPuppet.Wait();
            game.AddMovements(movement);
        }

        public void UpdateState() {
            Board board = game.State;
            foreach (IPacManClient pacManClient in pacManClients){
                pacManClient.UpdateState(board);
            }
        }

        public void UpdateConnectedClients() {
            foreach (IPacManClient pacManClient in pacManClients) {
                pacManClient.UpdateConnectedClients(clients);
            }
        }
    }
}