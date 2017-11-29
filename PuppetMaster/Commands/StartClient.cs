using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class StartClient : Command {
        public StartClient(PuppetMasterShell shell) : base(shell, "StartClient","StartClient PID PCS_URL CLIENT_URL MSEC_PER_ROUND NUM_PLAYERS ") {
        }

        public override void Execute(string[] args) {
            // Console.WriteLine("[PuppetMaster] STARTING SERVER");
            shell.pcs.createClient(args[0], "tcp://localhost:11000/ProcessCreationService", args[2], args[3], args[4]);
        }
    }
}
