using System.Timers;
using ClientServerInterface.PacMan.Server;
using OGPPacManClient.Interface;

namespace OGPPacManClient.Client {
    internal class MovementController {
        private readonly Form1 form;
        private readonly IPacmanServer server;
        private readonly Timer timer;
        private readonly int userId;

        public MovementController(Form1 form, IPacmanServer server, int delta, int userId) {
            this.userId = userId;
            this.form = form;
            this.server = server;
            timer = new Timer(delta) {AutoReset = true};
            timer.Elapsed += (sender, args) => NotifyServer();
        }

        public void Start() {
            timer.Start();
        }

        public void NotifyServer() {
            var dir = form.Direction;
            if (dir != Movement.Direction.Stopped){
                var mov = new Movement(userId, dir);
                server.SendAction(mov);
            }
        }
    }
}