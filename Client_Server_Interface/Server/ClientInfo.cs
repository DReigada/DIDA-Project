using System;

namespace ClientServerInterface.Server {
    [Serializable]
    public class ClientInfo {
        public ClientInfo(string url, string name) {
            Url = url;
            Name = name;
        }

        public string Url { get; }

        public string Name { get; }
    }
}