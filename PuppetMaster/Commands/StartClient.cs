using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class StartClient : Command {
        public static string masterURL;
        public StartClient(PuppetMasterShell shell) : base(shell, "StartClient","StartClient PID PCS_URL CLIENT_URL MSEC_PER_ROUND NUM_PLAYERS") {
        }

        public override void Execute(string[] args) {
            if(args.Length == 5){
                IProcesses procProxies;
                bool exists = shell.processes.TryGetValue(args[0], out procProxies);
                if (exists) {
                    Console.WriteLine("[StartClient] There is already a process with this id: \"{0}\"", args[0]);
                    return;
                }
                shell.connectPCS(args[1]).createClient(args[0], args[2], masterURL); // string clientIP, int clientPort, string serverURL

                String ip = args[2].Split('/')[2].Split(':')[0];
                String port = args[2].Split(':')[2].Split('/')[0];
                string url = $"tcp://{ip}:{port}/Puppet";

                System.Threading.Thread.Sleep(100);

                IProcesses client = (IProcesses) Activator.GetObject(typeof(IProcesses), url);

                shell.processes.Add(args[0], client);
                shell.processesURLs.Add(args[0], args[2]);
            }
            else if (args.Length == 6){ //has filename
                IProcesses procProxies;
                bool exists = shell.processes.TryGetValue(args[0], out procProxies);
                if (exists){
                    Console.WriteLine("[StartClient] There is already a process with this id: \"{0}\"", args[0]);
                    return;
                }
                shell.connectPCS(args[1]).createClient(args[0], args[2], args[5], masterURL);

                String ip = args[2].Split('/')[2].Split(':')[0];
                String port = args[2].Split(':')[2].Split('/')[0];
                string url = $"tcp://{ip}:{port}/Puppet";

                System.Threading.Thread.Sleep(100);

                IProcesses client = (IProcesses) Activator.GetObject(typeof(IProcesses), url);


                shell.processes.Add(args[0], client);
                shell.processesURLs.Add(args[0], args[2]);
            }
            else 
                printErrorMsg();
        }
    }
}
