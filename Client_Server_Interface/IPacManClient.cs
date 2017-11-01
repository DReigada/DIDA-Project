using System.Collections.Generic;

namespace Client_Server_Interface {
    public interface IPacManClient {
        void UpdateBoard(Board board);

        void UpdateConnectedClients(List<ConnectedClient> clients);
    }
}