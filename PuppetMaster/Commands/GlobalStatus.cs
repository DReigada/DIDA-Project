using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class GlobalStatus : Command{
        public GlobalStatus(PuppetMasterShell shell) : base(shell, "GlobalStatus","GlobalStatus") {
        }

        public override void Execute(string[] args) {
            if (args.Length != 0){
                printErrorMsg();
                return;
            }
            foreach (KeyValuePair<string , IProcesses> entry in shell.processes){
                try{
                    entry.Value.GlobalStatus();
                }
                catch (Exception e){
                    Console.WriteLine("[GlobalStatus] ERROR: {0}.", e.Message);
                }
            }
        }
    }
}

