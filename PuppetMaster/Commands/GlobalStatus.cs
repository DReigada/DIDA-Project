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
            /*foreach (IProcesses iprocess in shell.getAllOperators()) {
                new Thread(() => iprocess.GlobalStatus()).Start();
            }*/
        }
    }
}

