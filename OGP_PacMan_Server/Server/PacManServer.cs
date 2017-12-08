using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IList<ClientWithInfo<IPacManClient>> pacManClients;

        private readonly Timer proofTimer;

        private readonly List<ServerWithInfo<IPacManSlave>> servers;

        private readonly string url;

        private bool isMaster;

        private TimeSpan LastProof;

        private int pseudoMasterID;

        private int pseudoSlaveID;

        private int slaveID;

        //we should probably remove this one
        public PacManServer(int gameSpeed, int numberPlayers) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            clients = new List<ConnectedClient>();
            pacManClients = new List<ClientWithInfo<IPacManClient>>();
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
            pacManClients = new List<ClientWithInfo<IPacManClient>>();
            game = new PacManGame(numberPlayers);
            servers = new List<ServerWithInfo<IPacManSlave>>();

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
            pacManClients = new List<ClientWithInfo<IPacManClient>>();
            game = new PacManGame(numberPlayers);
            master = (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), masterUrl + "/PacManServer");
            servers = new List<ServerWithInfo<IPacManSlave>>();
            slaveID = 0;

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;

            proofTimer = new Timer();
            proofTimer.Elapsed += LifeProofEvent;
            proofTimer.Interval = this.gameSpeed;

            if (!isMaster) {
                var state = master.GetGameState(new SlaveInfo(url, false));
                if (state.Boards != null) game.StateHistory = state.Boards;
                foreach (var client in state.Clients) RegisterClient(new ClientInfo(client.Url));
                foreach (var movement in state.NewMovements) game.AddMovements(movement);
            }
        }

        public GameProps RegisterClient(ClientInfo client) {
            ServerPuppet.Instance.Wait();
            lock (pacManClients) {
                if (servers.Count != 0)
                    servers.AsParallel().ForAll(slave =>
                        new Thread(() => {
                            try {
                                slave.Server.RegisterClient(client);
                            }
                            catch (SocketException) {
                                slave.IsDead = true;
                            }
                        }).Start()
                    );

                var clientId = clients.Count + 1;
                clients.Add(new ConnectedClient(clients.Count + 1, client.Url));

                var pacManClient =
                    (IPacManClient) Activator.GetObject(typeof(IPacManClient), client.Url + "/PacManClient");

                var clientWithInfo = new ClientWithInfo<IPacManClient>(pacManClient, client.Url, clientId);
                pacManClients.Add(clientWithInfo);


                ThreadStart updateClient = UpdateConnectedClients;
                var updateThread = new Thread(updateClient);
                updateThread.Start();

                var props = new GameProps(gameSpeed, numberPlayers, clients.Count);
                if (clients.Count == numberPlayers) {
                    game.Start(clients);
                    new Thread(UpdateState).Start();
                    if (isMaster) gameTimer.Enabled = true;
                }
                return props;
            }
        }


        public void SendAction(Movement movement) {
            ServerPuppet.Instance.Wait();
            game.AddMovements(movement);
            if (servers.Count != 0)
                servers.AsParallel().ForAll(slave =>
                    new Thread(() => {
                        try {
                            slave.Server.SendAction(movement);
                        }
                        catch (SocketException) {
                            slave.IsDead = true;
                        }
                    }).Start()
                );
        }

        public void UpdateSlaveList(List<SlaveInfo> newServers) {
            if (slaveID == 0) slaveID = newServers.Count;
            foreach (var server in newServers) {
                var newServer = (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), server.Url + "/PacManServer");
                var serverInfo = new ServerWithInfo<IPacManSlave>(newServer, server.Url, false, server.Id);
                servers.Add(serverInfo);
            }
        }

        //FIXME: this one might be problematic
        public GameState GetGameState(SlaveInfo slaveInfo) {
            ServerPuppet.Instance.Wait();
            var gameState = new GameState(game.StateHistory, clients, game.NewMovements);
            var newSlave = (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), slaveInfo.Url + "/PacManServer");
            var server = new ServerWithInfo<IPacManSlave>(newSlave, slaveInfo.Url, false, servers.Count + 1);
            servers.Add(server);
            if (proofTimer.Enabled == false) proofTimer.Enabled = true;

            if (isMaster) {
                var allSlaves = new List<SlaveInfo>();
                foreach (var slave in servers) allSlaves.Add(new SlaveInfo(slave.URL, slave.IsDead, slave.Id));
                servers.AsParallel().ForAll(slave =>
                    new Thread(() => {
                        try {
                            slave.Server.UpdateSlaveList(allSlaves);
                        }
                        catch (SocketException) {
                            slave.IsDead = true;
                        }
                    }).Start()
                );
            }
            return gameState;
        }

        public void IAmAlive(TimeSpan time) {
            ServerPuppet.Instance.Wait();
            LastProof = time;
            if (proofTimer.Enabled == false) proofTimer.Enabled = true;
        }

        public void Kill() {
            Environment.Exit(1);
        }


        public void UpdateState() {
            ServerPuppet.Instance.Wait();
            var board = game.State;
            if (servers.Count != 0)
                servers.AsParallel().ForAll(slave =>
                    new Thread(() => {
                        try {
                            slave.Server.UpdateState();
                        }
                        catch (SocketException) {
                            slave.IsDead = true;
                        }
                    }).Start()
                );

            if (!isMaster) return;

            pacManClients.AsParallel().ForAll(pacManClient =>
                new Thread(() => {
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
                }).Start()
            );
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
            if (isMaster) {
                var time = DateTime.Now.TimeOfDay;
                if (servers.Count != 0)
                    foreach (var slave in servers)
                        if (slave.IsDead == false)
                            try {
                                slave.Server.IAmAlive(time);
                                break;
                            }
                            catch (SocketException) {
                                slave.IsDead = true;
                            }
                else proofTimer.Enabled = false;
            }
            else {
                //FIXME!!!
                Console.WriteLine(DateTime.Now.TimeOfDay.Subtract(LastProof));
                var diff = DateTime.Now.TimeOfDay.Subtract(LastProof);
                //Console.WriteLine(diff.TotalMilliseconds);
                if (diff.TotalMilliseconds > gameSpeed * 5) {
                    var firstAlive = true;
                    for (var i = slaveID - 2; i > 0; i++) {
                        if (!servers[i].IsDead) {
                            servers[i].IsDead = true;
                            firstAlive = false;
                            break;
                        }
                    }

                    if (firstAlive) {
                        isMaster = true;
                        gameTimer.Enabled = true;
                        Console.WriteLine(clients.Count);
                        TryToKillMaster();
                        foreach (var client in pacManClients) {
                            Console.WriteLine(url);
                            client.Client.UpdateServer(new ServerInfo(url));
                        }
                        Console.WriteLine(isMaster);
                    }
                }
                if (servers.Count > slaveID)
                    for (var i = slaveID; i < servers.Count; i++)
                        if (!servers[i].IsDead) {
                            var time = DateTime.Now.TimeOfDay;
                            servers[i].Server.IAmAlive(time);
                            break;
                        }
            }
        }

        public void UpdateSlave(Board board) {
            ServerPuppet.Instance.Wait();
            game.State = board;
        }

        public void UpdateConnectedClients() {
            ServerPuppet.Instance.Wait();
            lock (pacManClients) {
                foreach (var pacManClient in pacManClients)
                    try {
                        pacManClient.Client.UpdateConnectedClients(clients);
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

        private void TryToKillMaster() {
            new Thread(() => {
                try {
                    master.Kill();
                }
                catch (SocketException) {
                }
            }).Start();
        }
    }
}