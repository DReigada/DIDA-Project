using System;
using System.Collections.Generic;
using System.Linq;
using OGPPacManClient.Client.Chat.Order;

namespace OGPPacManClient.Client.Chat {
    [Serializable]
    internal class WrappedMessage<M> {
        public WrappedMessage(M message, int senderId, int messageId, (int, int)[] vectorClock) {
            Message = message;
            SenderId = senderId;
            IdsVectorClock = vectorClock.Select(a => a.Item1).ToArray();
            ClocksVectorClock = vectorClock.Select(a => a.Item2).ToArray();
            this.messageId = messageId;
        }

        public int[] IdsVectorClock { get; }
        public int[] ClocksVectorClock { get; }

        public M Message { get; }
        public int SenderId { get; }
        public int messageId { get; }

        public (int Id, int MsgId)[] VectorClock() {
            var clock = new (int Id, int MessageId)[IdsVectorClock.Length];
            for (var i = 0; i < IdsVectorClock.Length; i++) clock[i] = (IdsVectorClock[i], ClocksVectorClock[i]);
            return clock;
        }
    }

    internal class VectorClocks<M> : IMessager<M> {
        private readonly ReliableBroadcast<WrappedMessage<M>> broadcast;
        private readonly IDictionary<int, int> clocksVector;

        private readonly IDictionary<(int SenderId, int MsgId), WrappedMessage<M>> messagesToDeliver;
        private readonly int selfId;

        public VectorClocks(int selfId, string endpointName) {
            clocksVector = new Dictionary<int, int>();
            messagesToDeliver = new Dictionary<(int SenderId, int MsgId), WrappedMessage<M>>();
            broadcast = new ReliableBroadcast<WrappedMessage<M>>(selfId, endpointName);
            broadcast.ReceivedMessage += ReceiveMessage;
            this.selfId = selfId;
            clocksVector[selfId] = 0;
        }


        public event Action<M> ReceivedMessage;


        public void SendMessage(M message) {
            lock (this) {
                clocksVector[selfId] += 1;
                var wrap = new WrappedMessage<M>(message, selfId, clocksVector[selfId],
                    clocksVector.Select(a => (a.Key, a.Value)).ToArray());
                broadcast.SendMessage(wrap);
            }
        }

        public void AddClients(List<(int Id, string URL)> clients) {
            lock (this) {
                clients.ForEach(client => {
                    if (!clocksVector.ContainsKey(client.Id)) clocksVector[client.Id] = 0;
                });
                broadcast.AddClients(clients);
            }
        }

        public void Start() {
            broadcast.Start();
        }

        public IList<(int Id, string URL, bool isDead)> ListClientsInfo() {
            return broadcast.ListClientsInfo();
        }

        public void ReceiveMessage(WrappedMessage<M> message) {
            lock (this) {
                messagesToDeliver.Add((message.SenderId, message.messageId), message);
                DoDeliver();
            }
        }


        private void DoDeliver() {
            // C# does not like tail recursion :(
            while (true) {
                IList<(int, int)> delivered = new List<(int, int)>();
                foreach (var keyValuePair in messagesToDeliver)
                    if (IsDeliverable(keyValuePair.Value)) {
                        ReceivedMessage.Invoke(keyValuePair.Value.Message);
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
            return Array.TrueForAll(message.VectorClock(),
                messageClock =>
                    messageClock.Id == message.SenderId &&
                    messageClock.MsgId - 1 == clocksVector[messageClock.Id]
                    || messageClock.MsgId <= clocksVector[messageClock.Id]);
        }
    }
}