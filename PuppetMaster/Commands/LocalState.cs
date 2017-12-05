using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class LocalState : Command {
        public LocalState(PuppetMasterShell shell) : base(shell, "LocalState", "LocalState PID round_id") {
        }

        public override void Execute(string[] args){
            if (args.Length != 2){
                printErrorMsg();
                return;
            }
            string pid = args[0];
            int round_id = int.Parse(args[1]);
            IProcesses procProxies;
            bool isValid = shell.processes.TryGetValue(pid, out procProxies);
            if (!isValid) {
                Console.WriteLine("[LocalState] There isn't such Process Id: \"{0}\"", pid);
                return;
            }
            try{
                shell.processes[pid].LocalStatus(round_id);
            }
            catch (Exception e){
                Console.WriteLine("[LocalState] ERROR: {0}.", e.Message);
            }
        }
    }
}
