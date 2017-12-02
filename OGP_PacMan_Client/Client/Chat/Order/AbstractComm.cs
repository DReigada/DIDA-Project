using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OGPPacManClient.Client.Chat.Order {
    internal class WrappedMessage<M> {
        public WrappedMessage(M message, int senderId, int messageId) {
            Message = message;
            SenderId = senderId;
            this.messageId = messageId;
        }

        public M Message { get; }
        public int SenderId { get; }
        public int messageId { get; }
    }

    internal abstract class AbstractComm<M, Bla> : MarshalByRefObject where Bla : AbstractComm<M, Bla> {
        private const string EndpointName = "ClientChat";

        protected readonly IDictionary<int, Bla> otherClients;

        protected AbstractComm() {
            otherClients = new ConcurrentDictionary<int, Bla>();
        }

        public abstract void SendMessage(M message);

        protected abstract void ReceiveMessage(WrappedMessage<M> message);


        public void AddClients(List<Tuple<int, string>> clients) {
            lock (otherClients){
                clients.ForEach(client => {
                    if (!otherClients.ContainsKey(client.Item1)){
                        var newClient = ConnectToClient(client.Item2);
                        otherClients.Add(client.Item1, newClient);
                    }
                });
            }
        }

        public Bla ConnectToClient(string clientUrl) {
            return (Bla)
                Activator.GetObject(
                    typeof(Bla),
                    $"{clientUrl}/{EndpointName}");
        }
    }
}