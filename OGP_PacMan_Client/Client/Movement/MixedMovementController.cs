using ClientServerInterface.PacMan.Server;
using OGPPacManClient.Interface;

namespace OGPPacManClient.Client.Movement {
    internal class MixedMovementController : AbstractMovementController {
        private readonly FileMovementController fileController;
        private readonly MovementController movementController;
        private AbstractMovementController currentController;


        public MixedMovementController(string file, Form1 form, IPacmanServer server, int delta, int userId)
            : base(server, delta, userId) {
            fileController = new FileMovementController(file, server, delta, userId);
            movementController = new MovementController(form, server, delta, userId);
            currentController = fileController;
        }

        public override ClientServerInterface.PacMan.Server.Movement.Direction GetDirection() {
            if (currentController == fileController && !fileController.HasNext())
                currentController = movementController;

            return currentController.GetDirection();
        }
    }
}