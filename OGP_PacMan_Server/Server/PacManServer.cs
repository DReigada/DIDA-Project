using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Threading;
using System.Timers;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Game.PacMan;
using OGP_PacMan_Server.PuppetMaster;
using Timer = System.Timers.Timer;

namespace OGP_PacMan_Server.Server {
    public class PacManServer : MarshalByRefObject, IPacmanServer {
        private readonly PacManGame game;

        private readonly int gameSpeed;

        private readonly Timer gameTimer;

        private readonly string myUrl;

        private readonly int numberPlayers;

        private readonly IList<ClientWithInfo<IPacManClient>> pacManClients;

        private readonly List<ServerWithInfo<IPacmanServer>> pacmanServers;

        private readonly FaultTolerenceServer tolerenceServer;


        //we should probably remove this one
        public PacManServer(int gameSpeed, int numberPlayers) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            pacManClients = new List<ClientWithInfo<IPacManClient>>();
            game = new PacManGame(numberPlayers);

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;
        }

        public PacManServer(int gameSpeed, int numberPlayers, string url) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            myUrl = url;
            pacManClients = new List<ClientWithInfo<IPacManClient>>();
            game = new PacManGame(numberPlayers);
            pacmanServers = new List<ServerWithInfo<IPacmanServer>>();
            tolerenceServer = new FaultTolerenceServer(url, this.gameSpeed, 5, RegisterClient, UpdatePacManServers,
                updateClientsServer, RemovePacManServers);
            RemotingServices.Marshal(tolerenceServer, "FTServer", typeof(FaultTolerenceServer));

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;
        }

        public PacManServer(int gameSpeed, int numberPlayers, string url, string masterUrl) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            myUrl = url;
            pacManClients = new List<ClientWithInfo<IPacManClient>>();
            game = new PacManGame(numberPlayers);
            pacmanServers = new List<ServerWithInfo<IPacmanServer>>();
            tolerenceServer =
                new FaultTolerenceServer(url, this.gameSpeed, 5, RegisterClient, masterUrl, UpdatePacManServers,
                    updateClientsServer, RemovePacManServers);
            RemotingServices.Marshal(tolerenceServer, "FTServer", typeof(FaultTolerenceServer));

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;
        }

        public GameProps RegisterClient(ClientInfo client) {
            ServerPuppet.Instance.Wait();
            lock (pacManClients) {
                if (tolerenceServer.IsMaster) {
                    tolerenceServer.AddClient(client);
                    ThreadStart updateClient = UpdateConnectedClients;
                    var updateThread = new Thread(updateClient);
                    updateThread.Start();
                }

                var clientId = pacManClients.Count + 1;

                var pacManClient =
                    (IPacManClient) Activator.GetObject(typeof(IPacManClient), client.Url + "/PacManClient");

                var clientWithInfo = new ClientWithInfo<IPacManClient>(pacManClient, client.Url, clientId);
                pacManClients.Add(clientWithInfo);


                var props = new GameProps(gameSpeed, numberPlayers, pacManClients.Count);
                if (pacManClients.Count == numberPlayers) {
                    game.Start();
                    if (tolerenceServer.IsMaster) {
                        UpdatePacManServers(tolerenceServer.Servers);
                        gameTimer.Enabled = true;
                    }
                }
                return props;
            }
        }


        public void SendAction(Movement movement) {
            ServerPuppet.Instance.Wait();
            game.AddMovements(movement);
        }

        public void UpdateState(Board board) {
            ServerPuppet.Instance.Wait();
            if (tolerenceServer.IsMaster)
                new Thread(() => {
                    pacmanServers.AsParallel().ForAll(server => {
                        try {
                            server.Server.UpdateState(board);
                        }
                        catch (SocketException) {
                        }
                    });
                }).Start();
            else game.StateHistory.Add(board);
        }

        private void InitializePuppet() {
            ServerPuppet.Instance.GetRoundInfo = i => game.StateHistory.Find(b => b.RoundID == i).PrettyString();
        }

        public void UpdateClientBoard(Board board) {
            new Thread(() => {
                pacManClients.AsParallel().ForAll(pacManClient => {
                    try {
                        pacManClient.Client.UpdateState(board);
                        pacManClient.IsDead = false;
                    }
                    catch (SocketException) {
                        if (!pacManClient.IsDead) {
                            Console.WriteLine($"Client {pacManClient.Id} is dead");
                            pacManClient.IsDead = true;
                        }
                    }
                });
            }).Start();
        }

        private void UpdatePacManServers(List<ServerWithInfo<FaultTolerenceServer>> servers) {
            for (var i = 1; i < servers.Count; i++) {
                var pacmanServer =
                    (IPacmanServer) Activator.GetObject(typeof(IPacmanServer), servers[i].URL + "/PacManServer");
                pacmanServers.Add(new ServerWithInfo<IPacmanServer>(pacmanServer, servers[i].URL, false));
            }
        }

        private void RemovePacManServers(string url) {
            pacmanServers.RemoveAll(server => server.URL.Equals(url));
        }

        private void updateClientsServer() {
            foreach (var client in pacManClients) client.Client.UpdateServer(new ServerInfo(myUrl));
            gameTimer.Enabled = true;
        }

        private void TimeEvent(object source, ElapsedEventArgs e) {
            game.NextState();
            if (game.GameEnded) {
                gameTimer.Enabled = false;
                Console.WriteLine("GAME OVER!!!!");
            }
            var board = game.State();

            UpdateState(board);

            UpdateClientBoard(board);
        }

        public void UpdateConnectedClients() {
            ServerPuppet.Instance.Wait();
            lock (pacManClients) {
                var connectedClients = pacManClients
                    .Select(pacManClient => new ConnectedClient(pacManClient.Id, pacManClient.URL)).ToList();
                foreach (var pacManClient in pacManClients)
                    try {
                        pacManClient.Client.UpdateConnectedClients(connectedClients);
                        pacManClient.IsDead = false;
                    }
                    catch (SocketException) {
                        if (!pacManClient.IsDead) {
                            Console.WriteLine($"Client {pacManClient.Id} is dead");
                            pacManClient.IsDead = true;
                        }
                    }
            }
        }
    }
}