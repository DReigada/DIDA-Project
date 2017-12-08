using System;
using System.Collections.Generic;
using OGP_PacMan_Server.Server;

namespace OGP_PacMan_Server.Slave {
    public interface ISlave<TGameState> {
        void UpdateState();

        void UpdateSlaveList(List<SlaveInfo> servers);

        TGameState GetGameState(SlaveInfo slaveInfo);

        void IAmAlive(TimeSpan time);

        void Kill();
    }
}