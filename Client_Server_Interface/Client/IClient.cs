using System.Collections.Generic;

namespace ClientServerInterface.Client {
    public interface IClient<TState> {
        void UpdateState(TState board);

        void UpdateConnectedClients(List<ConnectedClient> clients);
    }
}