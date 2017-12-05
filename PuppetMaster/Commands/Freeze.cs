using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class Freeze : Command  {
        public Freeze(PuppetMasterShell shell) : base(shell, "Freeze", "Freeze PID") {
        }


        public override void Execute(string[] args) {
            if (args.Length != 1){
                printErrorMsg();
                return;
            }

            string pid = args[0];
            IProcesses procProxies;
            bool isValid = shell.processes.TryGetValue(pid, out procProxies);
            if (!isValid){
                Console.WriteLine("[Freeze] There isn't such Process Id: \"{0}\"", pid);
                return;
            }
            try {
                shell.processes[pid].Freeze();
            }
            catch (Exception e){
                Console.WriteLine("[Freeze] ERROR: {0}.", e.Message);
            }
        }
    }
}
