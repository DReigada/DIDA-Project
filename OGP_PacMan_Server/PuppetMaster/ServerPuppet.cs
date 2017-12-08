using System;
using OGPServices;

namespace OGP_PacMan_Server.PuppetMaster {
    public class ServerPuppet : BaseProcess {
        private static ServerPuppet _instance;

        private ServerPuppet() {
        }

        public static ServerPuppet Instance => _instance ?? (_instance = new ServerPuppet());

        public override void LocalStatus(int round_id) {
            throw new NotImplementedException();
        }
    }
}