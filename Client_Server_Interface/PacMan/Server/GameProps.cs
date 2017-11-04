using System;

namespace ClientServerInterface.PacMan.Server {
    [Serializable]
    public class GameProps {
        public GameProps(int gameSpeed, int numberPlayers)
        {
            GameSpeed = gameSpeed;
            NumberPlayers = numberPlayers;
        }

        public int GameSpeed { get; }

        public int NumberPlayers { get; }
    }
}