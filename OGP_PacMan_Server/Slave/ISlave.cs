using System;
using ClientServerInterface.Server;

namespace OGP_PacMan_Server.Slave {
    public interface ISlave<TAction, TGameState> {
        void UpdatSlaveClient(ClientInfo clientInfo);

        void SendSlaveAction(TAction action);

        TGameState GetGameState(SlaveInfo slaveInfo);

        void IAmAlive(TimeSpan time);

        void Kill();
    }
}