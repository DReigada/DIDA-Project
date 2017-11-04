using System;
using ClientServerInterface.Server;

namespace ClientServerInterface.PacMan.Server {
    public interface IPacmanServer : IServer<Movement, GameProps> {
    }
}