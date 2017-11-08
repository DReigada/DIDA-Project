using System.Collections.Generic;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGP_PacMan_Server.Game;

namespace OGP_PacMan_Server.Game {
    public class PacManGame : IGame {

        private int numberPlayers;

        private int playerSpeed = 5;

        private List<ClientInfo> clients;

        private List<Movement> newMovements = new List<Movement>();

        public Board Board { get; private set; }

        public bool GameEnded { get; private set; }

        private List<Coin> coins;

        private List<ServerGhost> ghosts;

        private List<ServerPacManPlayer> players;

        private List<Wall> walls;

        public PacManGame(int numberPlayer) {
            this.numberPlayers = numberPlayer;
            GameEnded = false;
        }

        public void Start(List<ClientInfo> clients) {
            int coinCounter = 1;

            List<Ghost> boardGhosts = new List<Ghost>();

            List<PacManPlayer> boardPlayers = new List<PacManPlayer>();

            this.clients = clients;

            //Inicialize Walls
            walls.Add(new Wall(1, new Position(288, 240)));
            walls.Add(new Wall(2, new Position(128, 240)));
            walls.Add(new Wall(3, new Position(248, 40)));
            walls.Add(new Wall(4, new Position(88, 40)));
            //Inicialize Ghosts
            ghosts.Add(new ServerGhost(GhostColor.Red, new Position(180, 73), 1, new Speed(5, 0)));
            ghosts.Add(new ServerGhost(GhostColor.Yellow, new Position(221, 273), 2, new Speed(5, 0)));
            ghosts.Add(new ServerGhost(GhostColor.Pink, new Position(301, 72), 3, new Speed(5, 5)));
            //Inicialize Players
            for (int i = 1; i <= this.clients.Count; i++) {
                players.Add(new ServerPacManPlayer(i, new Position(8, i * 40), 0, true, this.clients[i - 1].Name));
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
                boardPlayers.Add(new PacManPlayer(pacManPlayer.Id, pacManPlayer.Position, 0, true));
            }

            Board = new Board(boardGhosts, boardPlayers, coins);

        }

        public void NextState() {
            int deadCount = 0;

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

            foreach (ServerPacManPlayer player in players ){
                //Check if player is alive
                if (!player.Alive){
                    deadCount++;
                    continue;
                }

                //Checking coins
                foreach (Coin coin in coins) {
                    if (player.Position.X == coin.Position.X || player.Position.Y == coin.Position.Y || player.Alive) {
                        coins.RemoveAt(coin.Id - 1);
                        player.Score += 1;
                    }
                }
                //Wall Collision
                foreach (Wall wall in walls){
                    if ((wall.Position.X < player.Position.X) && (player.Position.X < wall.Position.X + wall.Width)) {
                        if ((wall.Position.Y < player.Position.Y) && (player.Position.Y < wall.Position.Y + wall.Length)){
                            player.Alive = false;
                            break;
                        }
                    }
                }

                //Ghost Collision
                foreach (ServerGhost ghost in ghosts){
                    if (player.Position.X == ghost.Position.X || player.Position.Y == ghost.Position.Y){
                        player.Alive = false;
                        break;
                    }
                }
            }

            if (deadCount == numberPlayers) {
                GameEnded = true;
            }
            
            foreach (ServerGhost ghost in ghosts) {
                boardGhosts.Add(new Ghost(ghost.Color, ghost.Position, ghost.Id));
            }

            foreach (ServerPacManPlayer pacManPlayer in players) {
                boardPlayers.Add(new PacManPlayer(pacManPlayer.Id, pacManPlayer.Position, 0, true));
            }

            Board = new Board(boardGhosts, boardPlayers, coins);
        }

        public void AddMovements(Movement movement) {
            newMovements.Add(movement);
        }
        
    }
}