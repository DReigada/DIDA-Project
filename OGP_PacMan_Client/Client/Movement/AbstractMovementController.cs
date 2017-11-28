using System.Timers;
using ClientServerInterface.PacMan.Server;

namespace OGPPacManClient.Client.Movement {
    public abstract class AbstractMovementController {
        private readonly IPacmanServer server;
        private readonly Timer timer;
        private readonly int userId;

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

        public void NotifyServer() {
            var dir = GetDirection();
            if (dir != ClientServerInterface.PacMan.Server.Movement.Direction.Stopped){
                var mov = new ClientServerInterface.PacMan.Server.Movement(userId, dir);
                server.SendAction(mov);
            }
        }
    }
}