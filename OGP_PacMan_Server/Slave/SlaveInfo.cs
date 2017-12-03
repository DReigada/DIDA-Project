using System;

namespace OGP_PacMan_Server.Slave {
    [Serializable]
    public class SlaveInfo {
        public SlaveInfo(string url) {
            Url = url;
        }

        public string Url { get; }
    }
}