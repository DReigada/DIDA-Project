using System;
using System.Collections.Generic;

namespace OGPPacManClient.Client.Chat.Order {
    public interface IMessager<M> {
        event Action<M> ReceivedMessage;

        void SendMessage(M message);

        void AddClients(List<(int Id, string URL)> clients);

        void Start();
    }
}