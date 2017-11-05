using System.Collections.Generic;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Game;

namespace OGP_PacMan_Server {
    public class PacManGame : IGame {

        private int numberPlayers;

        private int playerSpeed = 5;

        private List<ClientInfo> clients;

        private List<Movement> newMovements = new List<Movement>();

        public Board Board { get; private set; }

        private List<Coin> coins;

        private List<ServerGhost> ghosts;

        private List<ServerPacManPlayer> players;

        public PacManGame(int numberPlayer) {
        }

        public void Start(List<ClientInfo> clients) {
            int coinCounter = 1;

            List<Ghost> boardGhosts = new List<Ghost>();

            List<PacManPlayer> boardPlayers = new List<PacManPlayer>();

            this.clients = clients;
            //Inicialize Ghosts
            ghosts.Add(new ServerGhost(GhostColor.Red, new Position(180, 73), 1, new Speed(5, 0)));
            ghosts.Add(new ServerGhost(GhostColor.Yellow, new Position(221, 273), 2, new Speed(5, 0)));
            ghosts.Add(new ServerGhost(GhostColor.Pink, new Position(301, 72), 3, new Speed(5, 5)));
            //Inicialize Players
            for (int i = 1; i <= this.clients.Count; i++) {
                players.Add(new ServerPacManPlayer(i, new Position(8, i * 40), this.clients[i - 1].Name));
            }
            //Inicializa 1 Column of coins 
            for (int i = 1; i <= 8; i++) {
                coins.Add(new Coin(coinCounter++, new Position(48, i * 40)));
            }
            //Inicializa 2 Column of coins 
            for (int i = 1; i <= 8; i++){
                coins.Add(new Coin(coinCounter++, new Position(88, i * 40)));
            }
            //Inicializa 3 Column of coins 
            for (int i = 1; i <= 5; i++){
                coins.Add(new Coin(coinCounter++, new Position(128, i * 160)));
            }
            //Inicializa 4 Column of coins 
            for (int i = 1; i <= 5; i++){
                coins.Add(new Coin(coinCounter++, new Position(168, i * 40)));
            }
            //Inicializa 5 Column of coins
            for (int i = 1; i <= 8; i++){
                coins.Add(new Coin(coinCounter++, new Position(208, i * 40)));
            }
            //Inicializa 6 Column of coins
            for (int i = 1; i <= 8; i++) { 
                coins.Add(new Coin(coinCounter++, new Position(248, i * 40)));
            }
            //Inicializa 7 Column of coins
            for (int i = 1; i <= 5; i++){
                coins.Add(new Coin(coinCounter++, new Position(288, i * 160)));
            }
            //Inicializa 8 Column of coins
            for (int i = 1; i <= 5; i++){
                coins.Add(new Coin(coinCounter++, new Position(328, i * 40)));
            }
            //Inicializa 9 Column of coins
            for (int i = 1; i <= 8; i++){
                coins.Add(new Coin(coinCounter++, new Position(368, i * 40)));
            }

            foreach (ServerGhost ghost in ghosts){
                boardGhosts.Add(new Ghost(ghost.Color, ghost.Position, ghost.Id));
            }

            foreach (ServerPacManPlayer pacManPlayer in players){
                boardPlayers.Add(new PacManPlayer(pacManPlayer.Id, pacManPlayer.Position));
            }

            Board = new Board(boardGhosts, boardPlayers, coins);

        }

        public void NextState() {
            List<Ghost> boardGhosts = new List<Ghost>();

            List<PacManPlayer> boardPlayers = new List<PacManPlayer>();

            //Player Movements
            foreach (Movement movement in newMovements ){
                switch (movement.Direct){
                    case Movement.Direction.Down:
                        players[movement.Id].Position.Y -= playerSpeed;
                        break;
                    case Movement.Direction.Up:
                        players[movement.Id].Position.Y += playerSpeed;
                        break;
                    case Movement.Direction.Left:
                        players[movement.Id].Position.X -= playerSpeed;
                        break;
                    case Movement.Direction.Right:
                        players[movement.Id].Position.X += playerSpeed;
                        break;
                    case Movement.Direction.Stopped:
                        break;
                }
            }
            newMovements.Clear();

            //Ghost Movements
            foreach (ServerGhost ghost in ghosts){
                ghost.Position.X += ghost.Speed.X;
                ghost.Position.Y += ghost.Speed.Y;
            }

            //Checking coins
            foreach (Coin coin in coins ){
                foreach (ServerPacManPlayer player in players) {
                    if (player.Position.X == coin.Position.X || player.Position.Y == coin.Position.Y ) {
                        coins.RemoveAt(coin.Id - 1);
                    }
                }
            }
            //TODO: score and collisions

            foreach (ServerGhost ghost in ghosts)
            {
                boardGhosts.Add(new Ghost(ghost.Color, ghost.Position, ghost.Id));
            }

            foreach (ServerPacManPlayer pacManPlayer in players)
            {
                boardPlayers.Add(new PacManPlayer(pacManPlayer.Id, pacManPlayer.Position));
            }

            Board = new Board(boardGhosts, boardPlayers, coins);
        }

        public void AddMovements(Movement movement) {
            newMovements.Add(movement);
        }
        
    }
}