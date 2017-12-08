using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class InjectDelay : Command {
        public InjectDelay(PuppetMasterShell shell) : base(shell, "InjectDelay","InjectDelay src_PID dst_PID") {
        }

        public override void Execute(string[] args) {
            if (args.Length != 2){
                printErrorMsg();
                return;
            }

            IProcesses procProxies;
            string pid_src = args[0];
            string pid_dest = args[1];
            bool isValidSrc = shell.processes.TryGetValue(pid_src, out procProxies);
            if (!isValidSrc){
                Console.WriteLine("[InjectDelay] pid_src does not exist: \"{0}\"", pid_src);
                return;
            }
            bool isValidDest = shell.processes.TryGetValue(pid_dest, out procProxies);
            if (!isValidDest){
                Console.WriteLine("[InjectDelay] pid_dest does not exist: \"{0}\"", pid_dest);
                return;
            }

            try{
                shell.processesURLs.TryGetValue(pid_src, out var processURL);
                String ip = processURL.Split('/')[2].Split(':')[0];
                String port = processURL.Split(':')[2].Split('/')[0];
                string url = ip + ":" + port;

                shell.processes[pid_src].InjectDelay(url);
            }
            catch (Exception e){
                Console.WriteLine("[InjectDelay] ERROR: {0}.", e.Message);
            }
        }
    }
}
