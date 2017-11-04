using System;
using System.Collections.Generic;
using ClientServerInterface.PacMan.Client.Game;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;

namespace OGP_PacMan_Server {
    public class PacManServer : MarshalByRefObject, IPacmanServer {

        private PacManGame game;

        private List<ClientInfo> clients;

        private readonly int gameSpeed;

        private readonly int numberPlayers;

        public PacManServer(int gameSpeed, int numberPlayers) {
            this.gameSpeed = gameSpeed;

            this.numberPlayers = numberPlayers;

            clients = new List<ClientInfo>();

            game =  new PacManGame(numberPlayers);
        }

        public GameProps RegisterClient(ClientInfo client) {
            clients.Add(client);
            GameProps props = new GameProps(gameSpeed, numberPlayers);
            if (clients.Count == numberPlayers){
                game.Start(clients);
            }
            return props;
        }

        public void SendAction(Movement movement) {
            game.AddMovements(movement);
        }

    }
}