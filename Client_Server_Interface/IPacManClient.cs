using System.Collections.Generic;

namespace Client_Server_Interface {
    public interface IClient<TState> {
        void UpdateState(TState board);

        void UpdateConnectedClients(List<ConnectedClient> clients);
    }

    public interface IPacManClient : IClient<Board> {
    }
}