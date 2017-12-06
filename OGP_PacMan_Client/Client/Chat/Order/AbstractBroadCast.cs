using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting;
using OGPPacManClient.PuppetMaster;

namespace OGPPacManClient.Client.Chat.Order {
    [Serializable]
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

    internal abstract class AbstractBroadcast<M, Impl> : MarshalByRefObject, IMessager<M>
        where Impl : AbstractBroadcast<M, Impl> {
        private readonly string EndpointName;

        protected readonly IDictionary<int, Impl> OtherClients;
        protected readonly int selfId;

        protected AbstractBroadcast(int selfId, string endpointName) {
            EndpointName = endpointName;
            this.selfId = selfId;
            OtherClients = new ConcurrentDictionary<int, Impl>();
        }

        public event Action<M> ReceivedMessage;

        public abstract void SendMessage(M message);


        public void AddClients(List<(int Id, string URL)> clients) {
            lock (OtherClients){
                clients.ForEach(client => {
                    if (client.Id != selfId && !OtherClients.ContainsKey(client.Id)){
                        var newClient = ConnectToClient(client.URL);
                        OtherClients.Add(client.Id, newClient);
                    }
                });
            }
        }

        public void Start() {
            RemotingServices.Marshal(this, EndpointName);
        }

        protected void CallReceivedMessage(M m) {
            ReceivedMessage?.BeginInvoke(m, null, null);
        }

        // This should be protected but protected methods can't be called remotely
        public void ReceiveMessage(WrappedMessage<M> message) {
            ClientPuppet.Instance.Wait();
            DoReceiveMessage(message);
        }

        public abstract void DoReceiveMessage(WrappedMessage<M> message);


            public Impl ConnectToClient(string clientUrl) {
            return (Impl)
                Activator.GetObject(
                    typeof(Impl),
                    $"{clientUrl}/{EndpointName}");
        }
    }
}