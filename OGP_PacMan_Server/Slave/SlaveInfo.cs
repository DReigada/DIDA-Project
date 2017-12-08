using System;

namespace OGP_PacMan_Server.Slave {
    [Serializable]
    public class SlaveInfo {
        public SlaveInfo(string url, bool isDead) {
            Url = url;
            IsDead = isDead;
        }

        public SlaveInfo(string url, bool isDead, int id) {
            Url = url;
            IsDead = isDead;
            Id = id;
        }

        public string Url { get; }

        public bool IsDead { get; set; }

        public int Id { get; set; }
    }
}