﻿using System.Collections.Generic;
using ClientServerInterface.Client;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;

namespace OGP_PacMan_Server.Game {
    public interface IGame {
        void Start(List<ConnectedClient> clients);

        void NextState();

        void AddMovements(Movement movement);
    }
}