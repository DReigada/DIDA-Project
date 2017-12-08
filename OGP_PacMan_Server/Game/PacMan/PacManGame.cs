using System.Collections.Generic;
using System.Linq;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Client;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using OGP_PacMan_Server.Server;

namespace OGP_PacMan_Server.Game.PacMan {
    public class PacManGame : IGame<Board> {

        private readonly int coinSize = 22;

        private readonly int ghostSize = 30;

        private readonly int leftBorder = 0;

        private readonly int lowerBorder = 320;

        private readonly int numberPlayers;
        
        private readonly int playerSize = 25;

        private readonly int playerSpeed = 5;

        private readonly int rightBorder = 330;

        private readonly int topBorder = 40;

        private readonly List<Wall> walls;
        

        public PacManGame(int numberPlayer) {
            numberPlayers = numberPlayer;
            walls = new List<Wall>();
            NewMovements = new List<Movement>();
            StateHistory = new List<Board>();
            GameEnded = false;

            //Inicialize Walls
            walls.Add(new Wall(1, new Position(288, 240)));
            walls.Add(new Wall(2, new Position(128, 240)));
            walls.Add(new Wall(3, new Position(248, 40)));
            walls.Add(new Wall(4, new Position(88, 40)));
        }

        public List<Movement> NewMovements { get; }

        public List<Board> StateHistory { get; set; }
        
        public bool GameEnded { get; private set; }

        public void Start() {
            var coinCounter = 1;
           
            List<ServerGhost> ghosts = new List<ServerGhost>();

            List<PacManPlayer> players = new List<PacManPlayer>();

            List<Coin> coins = new List<Coin>();

            //Inicialize Ghosts
            ghosts.Add(new ServerGhost(GhostColor.Red, new Position(180, 73), 1, new Speed(5, 0)));
            ghosts.Add(new ServerGhost(GhostColor.Yellow, new Position(221, 273), 2, new Speed(5, 0)));
            ghosts.Add(new ServerGhost(GhostColor.Pink, new Position(301, 72), 3, new Speed(5, 5)));
            //Inicialize Players
            for (var i = 1; i <= numberPlayers; i++)
                players.Add(new PacManPlayer(i, new Position(8, i * 40), Movement.Direction.Stopped, 0, true));
            //Inicializa 1 Column of coins 
            for (var i = 1; i <= 8; i++) coins.Add(new Coin(coinCounter++, new Position(8, i * 40)));
            //Inicializa 2 Column of coins 
            for (var i = 1; i <= 8; i++) coins.Add(new Coin(coinCounter++, new Position(48, i * 40)));
            //Inicializa 3 Column of coins 
            for (var i = 1; i <= 5; i++) coins.Add(new Coin(coinCounter++, new Position(88, i * 40 + 120)));
            //Inicializa 4 Column of coins 
            for (var i = 1; i <= 5; i++) coins.Add(new Coin(coinCounter++, new Position(128, i * 40)));
            //Inicializa 5 Column of coins
            for (var i = 1; i <= 8; i++) coins.Add(new Coin(coinCounter++, new Position(168, i * 40)));
            //Inicializa 6 Column of coins
            for (var i = 1; i <= 8; i++) coins.Add(new Coin(coinCounter++, new Position(208, i * 40)));
            //Inicializa 7 Column of coins
            for (var i = 1; i <= 5; i++) coins.Add(new Coin(coinCounter++, new Position(248, i * 40 + 120)));
            //Inicializa 8 Column of coins
            for (var i = 1; i <= 5; i++) coins.Add(new Coin(coinCounter++, new Position(288, i * 40)));
            //Inicializa 9 Column of coins
            for (var i = 1; i <= 8; i++) coins.Add(new Coin(coinCounter++, new Position(328, i * 40)));
            
            StateHistory.Add(new Board(0, ghosts, players, coins));
        }

        public void NextState() {
            var deadCount = 0;

            var currentBoard = StateHistory.Last();

            var ghosts = GhostMovement(currentBoard.Ghosts);

            var newPlayers = new List<PacManPlayer>();

            var coins = new List<Coin>(currentBoard.Coins);

            foreach (var player in currentBoard.Players) {
                var newPlayer = player.Copy();

                //Check if player is alive
                if (!newPlayer.Alive) continue;

                PlayerMovement(newPlayer);

                var coin = CheckCoin(newPlayer, coins);

                if (coin != null) coins.Remove(coin);

                if (CheckPlayerWallCollision(newPlayer)) {
                    newPlayer.Alive = false;
                    deadCount++;
                    continue;
                }

                if (CheckPlayerGhostCollision(newPlayer, ghosts)) {
                    deadCount++;
                    newPlayer.Alive = false;
                }

                newPlayers.Add(newPlayer);
            }

            if (deadCount == numberPlayers) GameEnded = true;

            NewMovements.Clear();
            
            StateHistory.Add(new Board(currentBoard.RoundID + 1, ghosts, newPlayers, coins));
        }

        public Board State() {
            return StateHistory.Last();
        }

        public void AddMovements(Movement movement) {
            lock (NewMovements) {
                NewMovements.Add(movement);
            }
        }

        public List<ServerGhost> GhostMovement(List<ServerGhost> ghosts) {
            return ghosts.Select(ghost => {
                ServerGhost newGhost = ghost.Copy();
                newGhost.Position.X += newGhost.Speed.X;
                if (CheckGhostWallCollision(newGhost) || CheckGhostBorderCollision(newGhost)) {
                    newGhost.Position.X -= newGhost.Speed.X;
                    newGhost.Speed.X = -newGhost.Speed.X;
                }
                newGhost.Position.Y += newGhost.Speed.Y;
                if (CheckGhostWallCollision(newGhost) || CheckGhostBorderCollision(newGhost)) {
                    newGhost.Position.Y -= newGhost.Speed.Y;
                    newGhost.Speed.Y = -newGhost.Speed.Y;
                }
                return newGhost;
            }).ToList();
        }

        public void PlayerMovement(PacManPlayer player) {
            //Player Movements
            lock (NewMovements) {
                foreach (var movement in NewMovements)
                    if (movement.Id == player.Id) {
                        switch (movement.Direct) {
                            case Movement.Direction.Down:
                                player.Position.Y += playerSpeed;
                                if (CheckPlayerBorderCollision(player)) player.Position.Y -= playerSpeed;
                                break;
                            case Movement.Direction.Up:
                                player.Position.Y -= playerSpeed;
                                if (CheckPlayerBorderCollision(player)) player.Position.Y += playerSpeed;
                                break;
                            case Movement.Direction.Left:
                                player.Position.X -= playerSpeed;
                                if (CheckPlayerBorderCollision(player)) player.Position.X += playerSpeed;
                                break;
                            case Movement.Direction.Right:
                                player.Position.X += playerSpeed;
                                if (CheckPlayerBorderCollision(player)) player.Position.X -= playerSpeed;
                                break;
                            case Movement.Direction.Stopped:
                                break;
                        }
                        player.Direction = movement.Direct;
                    }
            }
        }

        public bool CheckPlayerWallCollision(PacManPlayer player) {
            //TODO:Test this
            foreach (var wall in walls)
                if (wall.Position.X - wall.Width <= player.Position.X &&
                    player.Position.X - playerSize <= wall.Position.X &&
                    wall.Position.Y <= player.Position.Y + playerSize &&
                    player.Position.Y <= wall.Position.Y + wall.Length) return true;
            return false;
        }

        public bool CheckGhostWallCollision(ServerGhost ghost) {
            foreach (var wall in walls)
                if (wall.Position.X - wall.Width <= ghost.Position.X &&
                    ghost.Position.X - ghostSize <= wall.Position.X &&
                    wall.Position.Y <= ghost.Position.Y + ghostSize &&
                    ghost.Position.Y <= wall.Position.Y + wall.Length) return true;
            return false;
        }

        public bool CheckPlayerGhostCollision(PacManPlayer player, List<ServerGhost> ghosts) {
            foreach (var ghost in ghosts)
                if (ghost.Position.X - ghost.Width <= player.Position.X &&
                    player.Position.X - playerSize <= ghost.Position.X &&
                    ghost.Position.Y <= player.Position.Y + playerSize &&
                    player.Position.Y <= ghost.Position.Y + ghost.Length) return true;
            return false;
        }

        public bool CheckPlayerBorderCollision(PacManPlayer player) {
            if (rightBorder < player.Position.X || player.Position.X < leftBorder || lowerBorder < player.Position.Y ||
                player.Position.Y < topBorder) return true;
            return false;
        }

        public bool CheckGhostBorderCollision(ServerGhost ghost) {
            if (rightBorder < ghost.Position.X || ghost.Position.X < leftBorder || lowerBorder < ghost.Position.Y ||
                ghost.Position.Y < topBorder) return true;
            return false;
        }

        public Coin CheckCoin(PacManPlayer player, List<Coin> coins) {
            foreach (var coin in coins)
                if (coin.Position.X - coinSize <= player.Position.X &&
                    player.Position.X - playerSize <= coin.Position.X &&
                    coin.Position.Y <= player.Position.Y + playerSize &&
                    player.Position.Y <= coin.Position.Y + coinSize) {
                    player.Score += 1;
                    return coin;
                }
            return null;
        }
    }
}