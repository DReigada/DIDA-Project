using System;
using System.Net.Sockets;
using System.Timers;
using ClientServerInterface.PacMan.Server;

namespace OGPPacManClient.Client.Movement {
    public abstract class AbstractMovementController {
        private readonly Timer timer;
        private readonly int userId;
        private IPacmanServer server;

        protected AbstractMovementController(IPacmanServer server, int delta, int userId) {
            this.userId = userId;
            this.server = server;
            timer = new Timer(delta) {AutoReset = true};
            timer.Elapsed += (sender, args) => NotifyServer();
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
                    server.SendAction(mov);
                }
            }
            catch (SocketException) {
                Console.WriteLine("Server is dead");
            }
        }

        public void setNewServer(IPacmanServer server) {
            Console.WriteLine("NEW SERVER");
            this.server = server;
        }
    }
}