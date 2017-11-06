using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class StartServer : Command{
        public StartServer(PuppetMasterShell shell) : base(shell, "StartServer","StartServer PID PCS_URL SERVER_URL MSEC_PER_ROUND NUM_PLAYERS") {
        }

        public override void Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
