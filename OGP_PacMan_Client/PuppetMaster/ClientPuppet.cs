using System;
using OGPServices;

namespace OGPPacManClient.PuppetMaster {
    public class ClientPuppet : BaseProcess {
        private static ClientPuppet _instance;

        private ClientPuppet() {
        }

        public static ClientPuppet Instance => _instance ?? (_instance = new ClientPuppet());

        public override void LocalStatus(int round_id) {
            throw new NotImplementedException();
        }
    }
}