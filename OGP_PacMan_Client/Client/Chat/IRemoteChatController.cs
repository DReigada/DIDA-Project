namespace OGPPacManClient.Client.Chat {
    internal interface IRemoteChatController {
        void ReceiveMessage(ChatMessage msg);
    }
}