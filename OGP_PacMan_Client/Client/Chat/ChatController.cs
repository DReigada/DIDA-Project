using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientServerInterface.Client;
using OGPPacManClient.Client.Chat.Order;

namespace OGPPacManClient.Client.Chat {
    internal class ChatController : MarshalByRefObject {
        private const string EndpointName = "ClientChat";

        private readonly IMessager<ChatMessage> messager;
        private readonly int SelfId;

        public ChatController(int selfId) {
            //messager = new ReliableBroadcast<ChatMessage>(selfId, EndpointName);
            messager = new VectorClocks<ChatMessage>(selfId, EndpointName);
            messager.ReceivedMessage += ReceiveMessage;
            messager.Start();

            SelfId = selfId;
        }


        public void ReceiveMessage(ChatMessage msg) {
            IncomingMessage?.BeginInvoke(msg.Content, null, null);
        }

        public event Action<string> IncomingMessage;


        public void SendMessage(string msg) {
            var message = new ChatMessage(SelfId, msg);
            new Thread(() => messager.SendMessage(message)).Start();
        }

        public void AddClients(List<ConnectedClient> clients) {
            messager.AddClients(clients.Select(c => (c.Id, c.Url)).ToList());
        }

        public IList<(int Id, string URL, bool isDead)> ListClientsInfo() {
            return messager.ListClientsInfo();
        }
    }
}