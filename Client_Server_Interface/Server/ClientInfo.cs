using System;

namespace ClientServerInterface.Server {
    [Serializable]
    public class ClientInfo {
        public ClientInfo(string url) {
            Url = url;
        }

        public string Url { get; }
    }
}