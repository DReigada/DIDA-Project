namespace ClientServerInterface.Server {
    public interface IInternalServer<TState> {
        void UpdateState(TState state);
    }
}