using OGP_PacMan_Server.Slave;

namespace OGP_PacMan_Server.Server {
    public class ServerWithInfo<TServer> {
        public ServerWithInfo(TServer server, string url, bool isMaster) {
            Server = server;
            URL = url;
            IsDead = false;
            IsMaster = isMaster;
        }
        
        public ServerInternalInfo GetInfo() {
            return new ServerInternalInfo(URL, IsDead, IsMaster);
        }

        public TServer Server { get; }
        public string URL { get; }
        public bool IsDead { get; set; }
        public bool IsMaster { get; set; }
    }
}