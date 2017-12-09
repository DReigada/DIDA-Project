using ClientServerInterface.PacMan.Server;
using OGPPacManClient.Interface;

namespace OGPPacManClient.Client.Movement {
    public class MovementController : AbstractMovementController {
        private readonly Form1 form;

        public MovementController(Form1 form, IPacmanServer server, string serverUrl, int delta, int userId) :
            base(server, serverUrl, delta, userId) {
            this.form = form;
        }

        public override ClientServerInterface.PacMan.Server.Movement.Direction GetDirection() {
            return form.Direction;
        }
    }
}
