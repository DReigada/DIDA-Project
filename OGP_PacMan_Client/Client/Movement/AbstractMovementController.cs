using System;
using System.Net.Sockets;
using System.Timers;
using ClientServerInterface.PacMan.Server;
using OGPPacManClient.PuppetMaster;

namespace OGPPacManClient.Client.Movement {
    public abstract class AbstractMovementController {
        private readonly Timer timer;
        private readonly int userId;
        private bool isServerDead;
        private IPacmanServer server;
        private string serverUrl;

        protected AbstractMovementController(IPacmanServer server, int delta, int userId) {
            this.userId = userId;
            this.server = server;
            timer = new Timer(delta) {AutoReset = true};
            timer.Elapsed += (sender, args) => NotifyServer();
            isServerDead = true;
        }

        public abstract ClientServerInterface.PacMan.Server.Movement.Direction GetDirection();

        public void Start() {
            timer.Start();
        }

        public void Stop() {
            timer.Stop();
        }

        public void NotifyServer() {
            try {
                var dir = GetDirection();
                if (dir != ClientServerInterface.PacMan.Server.Movement.Direction.Stopped) {
                    var mov = new ClientServerInterface.PacMan.Server.Movement(userId, dir);
                    ClientPuppet.Instance.DoDelay(serverUrl);
                    server.SendAction(mov);
                }
                isServerDead = false;
            }
            catch (SocketException) {
                isServerDead = true;
                Console.WriteLine("Server is dead");
            }
        }

        public void setNewServer(IPacmanServer server, string serverUrl) {
            Console.WriteLine("NEW SERVER");
            this.server = server;
            this.serverUrl = serverUrl;
            isServerDead = false;
        }

        public bool GetServerStatus() {
            return isServerDead;
        }
    }
}