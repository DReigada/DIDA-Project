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
                shell.connectPCS(args[1]).createServer(args[0], args[2], args[3], args[4]);
                IProcesses server = (IProcesses)Activator.GetObject(
                    typeof(IProcesses),
                    args[2]);
                //List<IProcesses> procProxies = new List<IProcesses>();
                //procProxies.Add(server);
                shell.processes.Add(args[0], server);
            }
            else
                printErrorMsg();
        }
    }
}
