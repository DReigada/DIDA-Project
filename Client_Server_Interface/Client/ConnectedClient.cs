using System;

namespace ClientServerInterface.Client {
    [Serializable]
    public class ConnectedClient {
        public ConnectedClient(int id, string url) {
            Id = id;
            Url = url;
        }

        public int Id { get; }
        public string Url { get; }
    }
}