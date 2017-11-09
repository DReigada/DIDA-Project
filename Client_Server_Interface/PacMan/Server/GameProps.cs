using System;

namespace ClientServerInterface.PacMan.Server {
    [Serializable]
    public class GameProps {
        public GameProps(int gameSpeed, int numberPlayers, int userId) {
            GameSpeed = gameSpeed;
            NumberPlayers = numberPlayers;
            UserId = userId;
        }

        public int UserId { get; set; }

        public int GameSpeed { get; }

        public int NumberPlayers { get; }
    }
}