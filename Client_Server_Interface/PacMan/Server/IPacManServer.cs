using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.Server;

namespace ClientServerInterface.PacMan.Server {
    public interface IPacmanServer : IServer<Movement, GameProps>, IInternalServer<Board> {
    }
}