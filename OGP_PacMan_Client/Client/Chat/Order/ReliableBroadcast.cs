using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

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


        public override void ReceiveMessage(WrappedMessage<M> message) {
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
                OtherClients.AsParallel().ForAll(a => sendMessageToClient(a, wrappedMessage))
            ).Start();
        }

        private void sendMessageToClient(
            KeyValuePair<int, ReliableBroadcast<M>> keyValue,
            WrappedMessage<M> wrappedMessage) {
            try {
                keyValue.Value.ReceiveMessage(wrappedMessage);
            }
            catch (SocketException) {
                Console.WriteLine($"Failed to send message to client: {keyValue.Key}");
            }
        }
    }
}