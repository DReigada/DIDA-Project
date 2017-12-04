using ClientServerInterface.PacMan.Server;

namespace OGP_PacMan_Server.Slave.PacMan {
    public interface IPacManSlave : IPacmanServer, ISlave<Movement, GameState> {
    }
}