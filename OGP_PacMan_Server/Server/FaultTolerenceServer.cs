using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Slave;
using Timer = System.Timers.Timer;

namespace OGP_PacMan_Server.Server {
    public class FaultTolerenceServer : MarshalByRefObject {
        private readonly int lifeCheckDelay;
        private readonly Timer lifeCheckTimer;
        private readonly Timer lifeProofTimer;
        private readonly string myUrl;
        private string personalMaster;
        private readonly Func<ClientInfo, GameProps> registerClient;
        private readonly Action setMaster;
        private readonly Action<List<ServerWithInfo<FaultTolerenceServer>>> updateServers;
        private readonly Action<String> removePacManServer;
        private TimeSpan lastProof;
        private FaultTolerenceServer personalSlave;

        public FaultTolerenceServer(string myUrl, int gameSpeed, int lifeCheckDelay,
            Func<ClientInfo, GameProps> registerClient,
            Action<List<ServerWithInfo<FaultTolerenceServer>>> updateServers, Action setMaster, Action<String> removePacManServer) {
            this.myUrl = myUrl;
            IsMaster = true;
            this.lifeCheckDelay = lifeCheckDelay * gameSpeed;
            this.registerClient = registerClient;
            this.updateServers = updateServers;
            this.setMaster = setMaster;
            this.removePacManServer = removePacManServer;
            Servers = new List<ServerWithInfo<FaultTolerenceServer>>();
            Servers.Add(new ServerWithInfo<FaultTolerenceServer>(null, myUrl, true));

            lifeProofTimer = new Timer();
            lifeProofTimer.Elapsed += (a, b) => SendLifeProof();
            lifeProofTimer.Interval = gameSpeed;

            lifeCheckTimer = new Timer();
            lifeCheckTimer.Elapsed += (a, b) => CheckLifeProof();
            lifeCheckTimer.Interval = gameSpeed * 5;
        }

        public FaultTolerenceServer(string myUrl, int gameSpeed, int lifeCheckDelay,
            Func<ClientInfo, GameProps> registerClient,
            string masterUrl, Action<List<ServerWithInfo<FaultTolerenceServer>>> updateServers,
            Action setMaster, Action<String> removePacManServer) {
            this.myUrl = myUrl;
            IsMaster = false;
            this.lifeCheckDelay = lifeCheckDelay * gameSpeed;
            this.registerClient = registerClient;
            this.updateServers = updateServers;
            this.setMaster = setMaster;
            this.removePacManServer = removePacManServer;
            lifeProofTimer = new Timer();
            lifeProofTimer.Elapsed += (a, b) => SendLifeProof();
            lifeProofTimer.Interval = gameSpeed;

            lifeCheckTimer = new Timer();
            lifeCheckTimer.Elapsed += (a, b) => CheckLifeProof();
            lifeCheckTimer.Interval = gameSpeed * 5;
            

            var master = (FaultTolerenceServer) Activator.GetObject(typeof(FaultTolerenceServer),
                masterUrl + "/FTServer");

            Servers = master.RegisterNewSlave(new ServerInternalInfo(myUrl, false, false));
            

            personalMaster = Servers[Servers.Count - 2].URL;
            

            foreach (var server in Servers.Take(Servers.Count - 1)) {
                server.Server =
                    (FaultTolerenceServer) Activator.GetObject(typeof(FaultTolerenceServer), server.URL + "/FTServer");


                if (server.URL.Equals(personalMaster)) {
                    server.Server.UpdateLifeProofSlave(myUrl);
                    //lifeCheckTimer.Enabled = true;
                }
            }
        }

        public List<ServerWithInfo<FaultTolerenceServer>> Servers { get; }

        public bool IsMaster { get; set; }


        public void AddClient(ClientInfo client) {
            if (IsMaster)
                new Thread(() => {
                    lock (Servers) {
                        Servers.AsParallel().ForAll(slave => {
                                try {
                                    if (!slave.IsMaster) slave.Server.AddClient(client);
                                }
                                catch (SocketException) {
                                    slave.IsDead = true;
                                }
                            }
                        );
                    }
                }).Start();
            else registerClient(client);
        }

        public void UpdateLifeProofSlave(string url) {
            //lock (personalSlave) {
            personalSlave = Servers.Find(server => server.URL.Equals(url)).Server;
            /*
            personalSlave =
                (FaultTolerenceServer) Activator.GetObject(typeof(FaultTolerenceServer), url + "/FTServer");*/
            if (lifeProofTimer.Enabled == false) {
                lifeProofTimer.Enabled = true;
            }
            //} 
        }

        public void RemoveServer(string url) {
            lock (Servers) {
                Servers.RemoveAll(server => server.URL.Equals(url));
                if (Servers[0].URL.Equals(myUrl) && !IsMaster) {
                    IsMaster = true;
                    lifeCheckTimer.Enabled = false;
                    setMaster();
                    updateServers(Servers);
                } 
                else if (!IsMaster && personalMaster.Equals(url)) {
                    int myIndex = Servers.FindIndex(server => server.URL.Equals(myUrl));
                    var newMaster = Servers[myIndex - 1];
                    personalMaster = newMaster.URL;
                    newMaster.Server.UpdateLifeProofSlave(myUrl);
                }
                else if (IsMaster) {
                    removePacManServer(url);
                }
            }
        }

        public List<ServerWithInfo<FaultTolerenceServer>> RegisterNewSlave(ServerInternalInfo serverInternalInfo) {
            lock (Servers) {
                var newSlave = (FaultTolerenceServer) Activator.GetObject(typeof(FaultTolerenceServer),
                    serverInternalInfo.Url + "/FTServer");
                Servers.Add(new ServerWithInfo<FaultTolerenceServer>(newSlave, serverInternalInfo.Url,
                    serverInternalInfo.IsMaster));
                new Thread(() => {
                    lock (Servers) {
                        Servers.AsParallel().ForAll(slave => {
                                try {
                                    if (!slave.IsMaster && !slave.URL.Equals(serverInternalInfo.Url))
                                        slave.Server.AddSlave(serverInternalInfo);
                                }
                                catch (SocketException) {
                                    slave.IsDead = true;
                                }
                            }
                        );
                    }
                }).Start();

                return Servers;
            }
        }

        public void AddSlave(ServerInternalInfo serverInternalInfo) {
            lock (Servers) {
                var newSlave = (FaultTolerenceServer) Activator.GetObject(typeof(FaultTolerenceServer),
                    serverInternalInfo.Url + "/FTServer");
                Servers.Add(new ServerWithInfo<FaultTolerenceServer>(newSlave, serverInternalInfo.Url,
                    serverInternalInfo.IsMaster));
            }
        }

        private void CheckLifeProof() {
            var diff = DateTime.Now.TimeOfDay.Subtract(lastProof).TotalMilliseconds;

            if (diff > lifeCheckDelay) {
                lifeCheckTimer.Enabled = false;
                var deadServer = Servers.Find(server => personalMaster.Equals(server.URL));
                TryToKill(deadServer.Server);
                RemoveServer(deadServer.URL);
                new Thread(() => {
                    lock (Servers) {
                        Servers.AsParallel().ForAll(slave => {
                                try {
                                    if (!slave.URL.Equals(myUrl)) slave.Server.RemoveServer(deadServer.URL);
                                }
                                catch (SocketException) {
                                    slave.IsDead = true;
                                }
                            }
                        );
                    }
                }).Start();
            }
        }

        public void SendLifeProof() {
            new Thread(() => {
                try {
                    personalSlave?.IAmAlive();
                }
                catch (SocketException) {
                }
            }).Start();
        }

        public void IAmAlive() {
            lastProof = DateTime.Now.TimeOfDay;
            lifeCheckTimer.Enabled = true;
        }

        private void TryToKill(FaultTolerenceServer server) {
            new Thread(() => {
                try {
                    server.Kill();
                }
                catch (SocketException) {
                }
            }).Start();
        }

        public void Kill() {
            Environment.Exit(1);
        }
    }
}