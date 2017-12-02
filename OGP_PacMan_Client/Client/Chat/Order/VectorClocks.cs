using System;
using System.Collections.Generic;
using System.Linq;
using OGPPacManClient.Client.Chat.Order;

namespace OGPPacManClient.Client.Chat {
    internal class WrappedMessage<M> {
        public WrappedMessage(M message, int senderId, int messageId, (int, int)[] vectorClock) {
            Message = message;
            SenderId = senderId;
            VectorClock = vectorClock;
            this.messageId = messageId;
        }

        public (int SenderId, int MsgId)[] VectorClock { get; }
        public M Message { get; }
        public int SenderId { get; }
        public int messageId { get; }
    }

    internal class VectorClocks<M> {
        private readonly ReliableBroadcast<WrappedMessage<M>> broadcast;
        private readonly IDictionary<int, int> clocksVector;
        private readonly Func<M, int> getSenderIdFunc;

        private readonly IDictionary<(int SenderId, int MsgId), WrappedMessage<M>> messagesToDeliver;
        private readonly int selfId;

        public VectorClocks(int selfId, Func<M, int> getSenderIdFunc) {
            clocksVector = new Dictionary<int, int>();
            messagesToDeliver = new Dictionary<(int SenderId, int MsgId), WrappedMessage<M>>();
            broadcast = new ReliableBroadcast<WrappedMessage<M>>(selfId);
            this.getSenderIdFunc = getSenderIdFunc;
            this.selfId = selfId;
        }

        public event Action<M> MessageReady;


        public void ReceiveMessage(WrappedMessage<M> message) {
            lock (this){
                messagesToDeliver.Add((message.SenderId, message.messageId), message);
                DoDeliver();
            }
        }


        public void SendMessage(M message) {
            lock (this){
                MessageReady.Invoke(message);
                clocksVector[selfId] += 1;
                var wrap = new WrappedMessage<M>(message, selfId, clocksVector[selfId],
                    clocksVector.Select(a => (a.Key, a.Value)).ToArray());
                broadcast.SendMessage(wrap);
            }
        }

        private void DoDeliver() {
            // C# does not like tail recursion :(
            while (true){
                IList<(int, int)> delivered = new List<(int, int)>();
                foreach (var keyValuePair in messagesToDeliver)
                    if (IsDeliverable(keyValuePair.Value)){
                        MessageReady.Invoke(keyValuePair.Value.Message);
                        delivered.Add(keyValuePair.Key);
                        clocksVector[keyValuePair.Key.SenderId] = keyValuePair.Key.MsgId;
                        break;
                    }

                // if a message was delivered we should check again if we can deliver more
                if (delivered.Count <= 0) break;
                foreach (var valueTuple in delivered) messagesToDeliver.Remove(valueTuple);
            }
        }

        private bool IsDeliverable(WrappedMessage<M> message) {
            return Array.TrueForAll(message.VectorClock,
                messageClock =>
                    messageClock.SenderId == message.SenderId &&
                    messageClock.MsgId - 1 <= clocksVector[messageClock.SenderId]
                    || messageClock.MsgId <= clocksVector[messageClock.SenderId]);
        }
    }
}