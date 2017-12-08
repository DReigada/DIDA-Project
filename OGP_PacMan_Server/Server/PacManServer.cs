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

        //private ServerWithInfo<IPacManSlave> slave;

        private readonly string myUrl;

        //private readonly IPacManSlave master;

        private readonly int numberPlayers;

        private readonly IList<ClientWithInfo<IPacManClient>> pacManClients;

        //private readonly Timer proofTimer;

        private readonly List<IPacmanServer> pacmanServers;

        private readonly FaultTolerenceServer tolerenceServer;

        //private bool isMaster;

        //private TimeSpan LastProof;

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
            pacmanServers = new List<IPacmanServer>();
            //FIXME
            tolerenceServer = new FaultTolerenceServer(url, this.gameSpeed, 5, RegisterClient, UpdatePacManServers, updateClientsServer);
            RemotingServices.Marshal(tolerenceServer, "FTServer", typeof(FaultTolerenceServer));

            /*selfID = 0;
            servers = new List<ServerWithInfo<IPacManSlave>>();
            servers.Add(new ServerWithInfo<IPacManSlave>(null, url, true));*/


            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;
            /*
            proofTimer = new Timer();
            proofTimer.Elapsed += LifeProofEvent;
            proofTimer.Interval = this.gameSpeed / 8;*/
        }

        public PacManServer(int gameSpeed, int numberPlayers, string url, string masterUrl) {
            this.gameSpeed = gameSpeed;
            this.numberPlayers = numberPlayers;
            myUrl = url;
            pacManClients = new List<ClientWithInfo<IPacManClient>>();
            game = new PacManGame(numberPlayers);
            pacmanServers = new List<IPacmanServer>();
            tolerenceServer =
                new FaultTolerenceServer(url, this.gameSpeed, 5, RegisterClient, masterUrl, UpdatePacManServers, updateClientsServer);
            RemotingServices.Marshal(tolerenceServer, "FTServer", typeof(FaultTolerenceServer));
            /*master = (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), masterUrl + "/PacManServer");
            servers = new List<ServerWithInfo<IPacManSlave>>();
            selfID = 0;*/

            gameTimer = new Timer();
            gameTimer.Elapsed += TimeEvent;
            gameTimer.Interval = gameSpeed;
            /*
            proofTimer = new Timer();
            proofTimer.Elapsed += LifeProofEvent;
            proofTimer.Interval = this.gameSpeed;*/

            /*if (!isMaster) {
                var state = master.GetGameState(new ServerInternalInfo(url, false));
                if (state.Boards != null) game.StateHistory = state.Boards;
                foreach (var client in state.Clients) RegisterClient(new ClientInfo(client.Url));
                foreach (var movement in state.NewMovements) game.AddMovements(movement);
            }*/
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
                    UpdatePacManServers(tolerenceServer.Servers);
                    if (tolerenceServer.IsMaster) gameTimer.Enabled = true;
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
                            server.UpdateState(board);
                        }
                        catch (SocketException) {
                        }
                    });
                }).Start();
            else game.StateHistory.Add(board);

        }

        public void UpdateClientBoard(Board board) {
            new Thread(() => {
                pacManClients.AsParallel().ForAll(pacManClient => {
                    try
                    {
                        pacManClient.Client.UpdateState(board);
                        pacManClient.IsDead = false;
                    }
                    catch (SocketException)
                    {
                        if (!pacManClient.IsDead)
                        {
                            Console.WriteLine($"Client {pacManClient.Id} is dead");
                            pacManClient.IsDead = true;
                        }
                    }
                });
            }).Start();
        }

        private void UpdatePacManServers(List<ServerWithInfo<FaultTolerenceServer>> servers) {
            Console.WriteLine("UPDATING SERVERS");
            Console.WriteLine(servers.Count);
            for (var i = 1; i < servers.Count; i++) {
                var pacmanServer =
                    (IPacmanServer) Activator.GetObject(typeof(IPacmanServer), servers[i].URL + "/PacManServer");
                pacmanServers.Add(pacmanServer);
            }
        }

        private void updateClientsServer() {
            foreach (var client in pacManClients) {
                Console.WriteLine("MY URL");
                Console.WriteLine(myUrl);
                client.Client.UpdateServer(new ServerInfo(myUrl));
            }
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


        /*
        public void UpdateSlaveList(List<ServerInternalInfo> newServers) {
            lock (servers) {
                var serversToAdd =
                    newServers.Where(newServer =>
                        !servers.Exists(server => server.URL == newServer.Url));
                foreach (var server in serversToAdd)
                    if (server.Url == url) {
                        selfID = newServers.Count - 1;
                        var serverInfo = new ServerWithInfo<IPacManSlave>(null, server.Url, server.IsMaster);
                        servers.Add(serverInfo);
                    }
                    else {
                        var newServer =
                            (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), server.Url + "/PacManServer");
                        var serverInfo = new ServerWithInfo<IPacManSlave>(newServer, server.Url, false);
                        servers.Add(serverInfo);
                    }
            }
        }

        public void RemoveServer(ServerInternalInfo serverToRemove) {
            lock (servers) {
                var id = servers.FindIndex(server => server.URL == serverToRemove.Url);
                servers.RemoveAt(id);
                if (selfID > id) selfID = id - 1;
            }
        }

        
        public GameState GetGameState(ServerInternalInfo serverInternalInfo) {
            ServerPuppet.Instance.Wait();
            lock (servers) {
                var gameState = new GameState(game.StateHistory, clients, game.NewMovements);
                var newSlave =
                    (IPacManSlave) Activator.GetObject(typeof(IPacManSlave), serverInternalInfo.Url + "/PacManServer");
                var server = new ServerWithInfo<IPacManSlave>(newSlave, serverInternalInfo.Url, false);
                servers.Add(server);
                if (proofTimer.Enabled == false) proofTimer.Enabled = true;

                if (isMaster) {
                    var allSlaves = new List<ServerInternalInfo>();
                    foreach (var slave in servers)
                        allSlaves.Add(new ServerInternalInfo(slave.URL, slave.IsDead, slave.IsMaster));
                    new Thread(() => {
                        servers.AsParallel().ForAll(slave => {
                            try {
                                if (!slave.IsMaster) slave.Server.UpdateSlaveList(allSlaves);
                            }
                            catch (SocketException) {
                                slave.IsDead = true;
                            }
                        });
                    }).Start();
                }
                return gameState;
            }
        }

        public void IAmAlive(TimeSpan time) {
            ServerPuppet.Instance.Wait();
            LastProof = time;
            if (proofTimer.Enabled == false) proofTimer.Enabled = true;
        }*/


        /*
        private void LifeProofEvent(object source, ElapsedEventArgs e) {
            if (isMaster) {
                var time = DateTime.Now.TimeOfDay;
                if (servers.Count > 1)
                    try {
                        servers[1].Server.IAmAlive(time);
                    }
                    catch (SocketException) {
                        servers[1].IsDead = true;
                    }
                else proofTimer.Enabled = false;
            }
            else {
                //FIXME!!!
                if (servers.Count > selfID) {
                    var time = DateTime.Now.TimeOfDay;
                    servers[selfID + 1].Server.IAmAlive(time);
                }

                Console.WriteLine(DateTime.Now.TimeOfDay.Subtract(LastProof));
                var diff = DateTime.Now.TimeOfDay.Subtract(LastProof);
                //Console.WriteLine(diff.TotalMilliseconds);
                if (diff.TotalMilliseconds > gameSpeed * 5) {
                    selfID = selfID - 1;
                    var deadServer = servers[selfID];
                    servers.RemoveAt(selfID);
                    if (selfID == 0) {
                        isMaster = true;
                        gameTimer.Enabled = true;
                        Console.WriteLine(clients.Count);
                        TryToKillMaster();
                        foreach (var client in pacManClients) {
                            Console.WriteLine(url);
                            client.Client.UpdateServer(new ServerInfo(url));
                        }
                    }
                    new Thread(() => {
                        servers.AsParallel().ForAll(slave => {
                            try {
                                if (slave.URL != url) slave.Server.RemoveServer(deadServer.GetInfo());
                            }
                            catch (SocketException) {
                                slave.IsDead = true;
                            }
                        });
                    }).Start();
                }*/


        /*var firstAlive = true;
                for (var i = selfID - 2; i > 0; i++)
                    if (!servers[i].IsDead) {
                        servers[i].IsDead = true;
                        firstAlive = false;
                        break;
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
            if (servers.Count > selfID)
                for (var i = selfID; i < servers.Count; i++)
                    if (!servers[i].IsDead) {
                        var time = DateTime.Now.TimeOfDay;
                        servers[i].Server.IAmAlive(time);
                        break;
                    }
    }
}

/*public void UpdateSlave(Board board) {
    ServerPuppet.Instance.Wait();
    game.State = board;
}*/
    }
}