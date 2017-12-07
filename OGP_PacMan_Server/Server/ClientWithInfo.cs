namespace OGP_PacMan_Server.Server {
    internal class ClientWithInfo<TClient> {
        public ClientWithInfo(TClient client, string url, int id) {
            Client = client;
            URL = url;
            IsDead = false;
            Id = id;
        }

        public TClient Client { get; }
        public string URL { get; }
        public int Id { get; }

        public bool IsDead { get; set; }
    }
}