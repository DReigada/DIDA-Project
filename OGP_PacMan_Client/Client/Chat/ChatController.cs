using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using ClientServerInterface.Client;
using OGPPacManClient.Interface;
//using OGPPacManClient.PuppetMaster;

namespace OGPPacManClient.Client.Chat {
    internal class ChatController : MarshalByRefObject, IRemoteChatController {
        private const string EndpointName = "ClientChat";
        private readonly IDictionary<int, IRemoteChatController> otherClients;
        private readonly int SelfId;

        private 

        public ChatController(int selfId) {
            SelfId = selfId;
            otherClients = new ConcurrentDictionary<int, IRemoteChatController>();
            RemotingServices.Marshal(this, EndpointName);
        }

        public void ReceiveMessage(ChatMessage msg) {
            //ClientPuppet.Wait();
            IncomingMessage?.BeginInvoke(msg.Content, null, null);
        }

        public event Action<string> IncomingMessage;

        public void SendMessage(string msg) {
            IncomingMessage?.Invoke(msg);
            var message = new ChatMessage(SelfId, msg);
            otherClients.AsParallel().ForAll(a => a.Value.ReceiveMessage(message));
        }

        public void AddClients(List<ConnectedClient> clients) {
            lock (otherClients){
                clients
                    .AsParallel()
                    .Where(a => a.Id != SelfId && !otherClients.ContainsKey(a.Id))
                    .Select(a => (Id: a.Id, Client: ConnectToClient(a)))
                    .ForAll(a => otherClients.Add(a.Id, a.Client));
            }
        }

        public IRemoteChatController ConnectToClient(ConnectedClient client) {
            return (IRemoteChatController)
                Activator.GetObject(
                    typeof(IRemoteChatController),
                    $"{client.Url}/{EndpointName}");
        }
    }
}