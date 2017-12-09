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
                IProcesses procProxies;
                bool exists = shell.processes.TryGetValue(args[0], out procProxies);
                if (exists)   {
                    Console.WriteLine("[StartServer] There is already a process with this id: \"{0}\"", args[0]);
                    return;
                }

                String ip = args[2].Split('/')[2].Split(':')[0];
                String port = args[2].Split(':')[2].Split('/')[0];
                string url = $"tcp://{ip}:{port}/Puppet";

                if (StartClient.masterURL == null) {
                    StartClient.masterURL = $"{ip}:{port}";
                }
                shell.connectPCS(args[1]).createServer(args[0], args[2], args[3], args[4]);

              

                //System.Threading.Thread.Sleep(100);

                IProcesses server = (IProcesses) Activator.GetObject(typeof(IProcesses), url);

                shell.processes.Add(args[0], server);
                shell.processesURLs.Add(args[0], args[2]);
            }
            else
                printErrorMsg();
        }
    }
}
