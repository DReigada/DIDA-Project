using System;
using System.Collections.Generic;
using System.Linq;
using OGPServices;

namespace OGPPacManClient.PuppetMaster {
    public class ClientPuppet : BaseProcess {
        private static ClientPuppet _instance;

        private ClientPuppet() {
            ListClientsInfo = () => new List<(int Id, string URL, bool isDead)>();
            ListServersInfo = () => new List<(int Id, string URL, bool isDead)>();
        }

        public Func<IList<(int Id, string URL, bool isDead)>> ListClientsInfo { get; set; }
        public Func<IList<(int Id, string URL, bool isDead)>> ListServersInfo { get; set; }


        public static ClientPuppet Instance => _instance ?? (_instance = new ClientPuppet());


        public override void InjectDelay(string pid_dest) {
            throw new NotImplementedException();
        }

        public override void GlobalStatus() {
            var clients = string.Join("\n", ListClientsInfo().Select(createString));
            var servers = string.Join("\n", ListServersInfo().Select(createString));

            Console.WriteLine($"GlobalStatus\nClients:\n{clients}\n Servers:\n{servers}");
        }

        public string createString((int Id, string URL, bool isDead) tuple) {
            return $"{tuple.Id} {tuple.URL}, {tuple.isDead}";
        }

        public override void LocalStatus(int round_id) {
            throw new NotImplementedException();
        }
    }
}