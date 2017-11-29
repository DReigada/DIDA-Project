using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class StartServer : Command{
        public StartServer(PuppetMasterShell shell) : base(shell, "StartServer","StartServer PID PCS_URL SERVER_URL MSEC_PER_ROUND NUM_PLAYERS") {
        }

        public override void Execute(string[] args) {
            if (args.Length == 5) {
               // Console.WriteLine("[PuppetMaster] STARTING SERVER");
               shell.pcs.createServer(args[0], args[1], args[2], args[3], args[4]);
            }
            if (args.Length == 6){ //filename
               // Console.WriteLine("[PuppetMaster] STARTING SERVER");
                //shell.pcs.createServer(args[0], "tcp://localhost:11000/ProcessCreationService", args[2], args[3], args[4], args[5]);
            }
        }
    }
}
