using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class StartClient : Command {
        public StartClient(PuppetMasterShell shell) : base(shell, "StartClient","StartClient PID PCS_URL CLIENT_URL MSEC_PER_ROUND NUM_PLAYERS ") {
        }

        public override void Execute(string[] args) {
            if(args.Length == 5){
                shell.connectPCS(args[1]).createClient(args[0], args[2]); // string clientIP, int clientPort, string serverURL
                IProcesses client = (IProcesses) Activator.GetObject(typeof(IProcesses), args[2]);
                //List<IProcesses> procProxies = new List<IProcesses>();
                //procProxies.Add(client);
                shell.processes.Add(args[0], client);
            }
            else if (args.Length == 6){ //has filename
                shell.connectPCS(args[1]).createClientWithFilename(args[0], args[2], args[5]);
                IProcesses client = (IProcesses)Activator.GetObject(typeof(IProcesses), args[2]);
            }
            else 
                printErrorMsg();
        }
    }
}
