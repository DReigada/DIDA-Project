using System;
using System.Collections.Generic;
using ClientServerInterface.PacMan.Client.Game;
using OGP_PacMan_Server.Server;

namespace OGP_PacMan_Server.Slave {
    public interface ISlave<TGameState> {
        void UpdateState(Board board);

        void UpdateSlaveList(List<ServerInternalInfo> servers);

        TGameState GetGameState(ServerInternalInfo serverInternalInfo);

        void RemoveServer(ServerInternalInfo serverInternalInfo);

        void IAmAlive(TimeSpan time);

        void Kill();
    }
}