namespace ClientServerInterface.Server {
    public class ClientInfo {
        public ClientInfo(string port, string name)
        {
            Port = port;
            Name = name;
        }

        public string Port { get; }

        public string Name { get; }

    }
}