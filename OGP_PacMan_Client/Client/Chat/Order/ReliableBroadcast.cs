using System;
using System.Collections.Generic;
using System.Linq;

namespace OGPPacManClient.Client.Chat.Order {
    internal class ReliableBroadcast<M> : AbstractComm<M, ReliableBroadcast<M>> {
        private readonly ISet<(int, int)> seenMessages;

        private readonly int selfId;
        private int counter;

        public ReliableBroadcast(int selfId) {
            this.selfId = selfId;
            counter = 0;
            seenMessages = new HashSet<(int, int)>();
        }

        public event Action<M> ReceivedMessage;

        public override void SendMessage(M message) {
            lock (this){
                counter++;
                var wrappedMessage = new WrappedMessage<M>(message, selfId, counter);
                var msgId = (wrappedMessage.SenderId, wrappedMessage.messageId);
                seenMessages.Add(msgId);

                DoSendMessage(wrappedMessage);
            }
        }


        protected override void ReceiveMessage(WrappedMessage<M> message) {
            lock (this){
                ReceivedMessage?.BeginInvoke(message.Message, null, null);

                var msgId = (message.SenderId, message.messageId);

                if (!seenMessages.Contains(msgId)){
                    seenMessages.Add(msgId);
                    DoSendMessage(message);
                }
            }
        }


        private void DoSendMessage(WrappedMessage<M> wrappedMessage) {
            otherClients.AsParallel()
                .ForAll(a => a.Value.ReceiveMessage(wrappedMessage));
        }
    }
}