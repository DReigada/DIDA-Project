using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Game.PacMan;
using OGP_PacMan_Server.PuppetMaster;
using OGP_PacMan_Server.Slave;
using OGP_PacMan_Server.Slave.PacMan;
using Timer = System.Timers.Timer;

namespace OGP_PacMan_Server.Server {
    public class PacManServer : MarshalByRefObject, IPacManSlave {
        private readonly List<ConnectedClient> clients;

        private readonly PacManGame game;

        private readonly int gameSpeed;

        private readonly Timer gameTimer;

        private readonly IPacManSlave master;

        private readonly int numberPlayers;

        private readonly List<IPacManClient> pacManClients;

        private readonly Timer proofTimer;

        private IPacManSlave slave;

        private readonly string url;

        private bool isMaster;

        private TimeSpan LastProof;

        public PacManServer(int gameSpeed, int numberPlayers) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            clients = new List<ConnectedClient>();
            pacManClients = new List<IPacManClient>();
            game = new PacManGame(numberPlayers);

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;
        }

        public PacManServer(int gameSpeed, int numberPlayers, string url, bool isMaster) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            this.isMaster = isMaster;
            this.url = url;
            clients = new List<ConnectedClient>();
            pacManClients = new List<IPacManClient>();
            game = new PacManGame(numberPlayers);

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;

            proofTimer = new Timer();
            proofTimer.Elapsed += LifeProofEvent;
            proofTimer.Interval = this.gameSpeed / 8;
        }

        public PacManServer(int gameSpeed, int numberPlayers, string url, string masterUrl, bool isMaster) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            this.isMaster = isMaster;
            this.url = url;
            clients = new List<ConnectedClient>();
            pacManClients = new List<IPacManClient>();
            game = new PacManGame(numberPlayers);
            master = (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), masterUrl + "/PacManServer");

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;

            proofTimer = new Timer();
            proofTimer.Elapsed += LifeProofEvent;
            proofTimer.Interval = this.gameSpeed;

            if (!isMaster) {
                var state = master.GetGameState(new SlaveInfo(url));
                if (state.Board != null) game.State = state.Board;
                foreach (var client in state.Clients) RegisterClient(new ClientInfo(client.Url));
                foreach (var movement in state.NewMovements) game.AddMovements(movement);
            }
        }

        public GameProps RegisterClient(ClientInfo client) {
            ServerPuppet.Wait();
            lock (pacManClients) {
                if(slave != null) { 
                    Console.WriteLine("here");
                    slave.RegisterClient(client);
                }
                clients.Add(new ConnectedClient(clients.Count + 1, client.Url));

                var pacManClient =
                    (IPacManClient) Activator.GetObject(typeof(IPacManClient), client.Url + "/PacManClient");
                pacManClients.Add(pacManClient);


                ThreadStart updateClient = UpdateConnectedClients;
                var updateThread = new Thread(updateClient);
                updateThread.Start();

                var props = new GameProps(gameSpeed, numberPlayers, clients.Count);
                if (clients.Count == numberPlayers) {
                    game.Start(clients);
                    ThreadStart theardStart = UpdateState;
                    var thread = new Thread(theardStart);
                    thread.Start();
                    UpdateState();
                    gameTimer.Enabled = true;
                }
                return props;
            }
        }

        public void SendAction(Movement movement) {
            ServerPuppet.Wait();
            game.AddMovements(movement);
            if (slave != null) {
                slave.SendAction(movement);
            }
        }

        //will probably remove this
        public void UpdatSlaveClient(ClientInfo clientInfo) {
        }

        //will probably remove this
        public void SendSlaveAction(Movement movement) {
        }

        public GameState GetGameState(SlaveInfo slaveInfo) {
            ServerPuppet.Wait();
            var gameState = new GameState(game.State, clients, game.NewMovements);
            slave = (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), slaveInfo.Url + "/PacManServer");
            if (proofTimer.Enabled == false) proofTimer.Enabled = true;
            return gameState;
        }

        public void IAmAlive(TimeSpan time) {
            ServerPuppet.Wait();
            LastProof = time;
            if (proofTimer.Enabled == false) proofTimer.Enabled = true;
        }

        public void LifeProof() {
            ServerPuppet.Wait();
            if (isMaster) {
                var time = DateTime.Now.TimeOfDay;
                if (slave != null) //Console.WriteLine(time);
                    try {
                        slave.IAmAlive(time);
                    }
                    catch (SocketException) {
                        slave = null;
                        proofTimer.Enabled = false;
                    }
            }
            else {
                Console.WriteLine(DateTime.Now.TimeOfDay.Subtract(LastProof));
                var diff = DateTime.Now.TimeOfDay.Subtract(LastProof);
                //Console.WriteLine(diff.TotalMilliseconds);
                if (diff.TotalMilliseconds > gameSpeed * 5) {
                    isMaster = true;
                    Console.WriteLine(clients.Count);
                    master.Kill();
                    foreach (var client in pacManClients) {
                        Console.WriteLine(url);
                        client.UpdateServer(new ServerInfo(url));
                    }
                    proofTimer.Enabled = false;
                    Console.WriteLine(isMaster);
                }
            }
        }

        private void TimeEvent(object source, ElapsedEventArgs e) {
            game.NextState();
            if (game.GameEnded) {
                gameTimer.Enabled = false;
                Console.WriteLine("GAME OVER!!!!");
            }
            UpdateState();
        }

        private void LifeProofEvent(object source, ElapsedEventArgs e) {
            LifeProof();
        }

        public void UpdateState() {
            ServerPuppet.Wait();
            var board = game.State;
            try {
                if (isMaster) foreach (var pacManClient in pacManClients) pacManClient.UpdateState(board);
            }
            catch (SocketException) {
            }
        }

        public void UpdateSlave(Board board) {
            ServerPuppet.Wait();
            game.State = board;
        }

        public void UpdateConnectedClients() {
            ServerPuppet.Wait();
            lock (pacManClients) {
                foreach (var pacManClient in pacManClients) pacManClient.UpdateConnectedClients(clients);
            }
        }

        public void Kill() {
            Environment.Exit(1);
        }
    }
}