using System.Collections.Generic;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Server;

namespace OGP_PacMan_Server.Game {
    public interface IGame<TState> {
        bool GameEnded { get; }

        List<TState> StateHistory { get; set; }

        void Start(List<ConnectedClient> clients);

        void NextState();

        void AddMovements(Movement movement);
    }
}