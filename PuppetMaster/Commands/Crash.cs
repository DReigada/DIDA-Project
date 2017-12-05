using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class Crash : Command {
        public Crash(PuppetMasterShell shell) : base(shell, "Crash","Crash PID") {
        }

        public override void Execute(string[] args) {
            if (args.Length != 1) {
                printErrorMsg();
                return;
            }
            string pid = args[0];
            IProcesses procProxies;
            bool isValid = shell.processes.TryGetValue(pid, out procProxies);
            if (!isValid){
                Console.WriteLine("[Crash] There isn't such Process Id: \"{0}\"", pid);
                return;
            }
            try{
                shell.processes[pid].Crash();
            }
            catch (Exception e){
                Console.WriteLine("[Crash] ERROR: {0}.", e.Message);
            }
        }
    }
}
