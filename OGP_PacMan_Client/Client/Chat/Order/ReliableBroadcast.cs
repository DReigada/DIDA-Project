using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using OGPPacManClient.PuppetMaster;

namespace OGPPacManClient.Client.Chat.Order {
    internal class ReliableBroadcast<M> : AbstractBroadcast<M, ReliableBroadcast<M>> {
        private readonly ISet<(int, int)> seenMessages;

        private int counter;

        public ReliableBroadcast(int selfId, string endpointName) : base(selfId, endpointName) {
            counter = 0;
            seenMessages = new HashSet<(int, int)>();
        }


        public override void SendMessage(M message) {
            lock (this) {
                CallReceivedMessage(message);
                counter++;
                var wrappedMessage = new WrappedMessage<M>(message, selfId, counter);
                var msgId = (wrappedMessage.SenderId, wrappedMessage.messageId);
                seenMessages.Add(msgId);

                DoSendMessage(wrappedMessage);
            }
        }


        public override void DoReceiveMessage(WrappedMessage<M> message) {
            lock (this) {
                var msgId = (message.SenderId, message.messageId);

                if (!seenMessages.Contains(msgId)) {
                    CallReceivedMessage(message.Message);
                    seenMessages.Add(msgId);
                    new Thread(() => DoSendMessage(message)).Start();
                }
            }
        }


        private void DoSendMessage(WrappedMessage<M> wrappedMessage) {
            new Thread(() =>
                OtherClients.AsParallel().ForAll(c => {
                    sendMessageToClient(c.Value, wrappedMessage);
                })
            ).Start();
        }

        private void sendMessageToClient(
            ClientWithInfo<ReliableBroadcast<M>> client,
            WrappedMessage<M> wrappedMessage) {
            try {
                ClientPuppet.Instance.DoDelay(client.URL);
                client.Client.ReceiveMessage(wrappedMessage);
                client.IsDead = false;
            }
            catch (SocketException) {
                client.IsDead = true;
                Console.WriteLine($"Failed to send message to client: {client.Id}");
            }
        }
    }
}