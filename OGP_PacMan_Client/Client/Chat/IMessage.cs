using System;

namespace OGPPacManClient.Client.Chat {
    internal interface IMessage<out TContent> {
        int SenderId { get; }
        TContent Content { get; }
    }

    [Serializable]
    internal class ChatMessage : IMessage<string> {
        public ChatMessage(int senderId, string content) {
            SenderId = senderId;
            Content = content;
        }

        public int SenderId { get; }
        public string Content { get; }
    }
}