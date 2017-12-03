using System;

namespace ClientServerInterface.Client {
    [Serializable]
    public class ServerInfo {
        public ServerInfo(string url) {
            Url = url;
        }

        public string Url { get; }
    }
}