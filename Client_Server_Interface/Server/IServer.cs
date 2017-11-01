namespace ClientServerInterface.Server {
    public interface IServer<TAction, TGameProps> {
        TGameProps RegisterClient(ClientInfo client);

        void SendAction(TAction action);
    }

}