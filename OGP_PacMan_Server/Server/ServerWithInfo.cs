namespace OGP_PacMan_Server.Server {
    public class ServerWithInfo<TServer> {
        public ServerWithInfo(TServer server, string url, bool isMaster) {
            Server = server;
            URL = url;
            IsDead = false;
            IsMaster = isMaster;
        }

        public ServerWithInfo(TServer server, string url, bool isMaster, int id) {
            Server = server;
            URL = url;
            IsDead = false;
            IsMaster = isMaster;
            Id = id;
        }

        public TServer Server { get; }
        public string URL { get; }
        public bool IsDead { get; set; }
        public bool IsMaster { get; set; }
        public int Id { get; set; }
    }
}