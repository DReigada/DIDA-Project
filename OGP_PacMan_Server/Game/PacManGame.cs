using System.Collections.Generic;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Game;

namespace OGP_PacMan_Server {
    public class PacManGame : IGame {

        private int numberPlayers;

        private Board board;

        private List<ClientInfo> clients;

        private List<ServerGhost> ghosts;

        private List<ServerPacManPlayer> players;

        private readonly List<Movement> newMovements = new List<Movement>();

        public PacManGame(int numberPlayer) {
        }

        public void Start(List<ClientInfo> clients) {
            
        }

        public void NextState() {
        }

        public void AddMovements(Movement movement) {
            newMovements.Add(movement);
        }
    }
}