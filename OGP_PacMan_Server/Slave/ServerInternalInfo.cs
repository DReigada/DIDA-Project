using System;

namespace OGP_PacMan_Server.Slave {
    [Serializable]
    public class ServerInternalInfo {
        public ServerInternalInfo(string url, bool isDead) {
            Url = url;
            IsDead = isDead;
        }

        public ServerInternalInfo(string url, bool isDead, bool isMaster) {
            Url = url;
            IsDead = isDead;
            IsMaster = isMaster;
        }

        public string Url { get; }

        public bool IsDead { get; set; }

        public bool IsMaster { get; set; }
        
    }
}