using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class Freeze : Command  {
        public Freeze(PuppetMasterShell shell) : base(shell, "Freeze", "Freeze PID") {
        }


        public override void Execute(string[] args) {
            if (args.Length != 1) { //just for test, must be 1
                printErrorMsg();
                return;
            }

            List<IProcesses> pmProcessesList;
            string pid = args[0];
            bool tryGetValue = shell.processesProxies.TryGetValue(pid, out pmProcessesList);
            if (tryGetValue) {
                printErrorMsg();
                return;
            }


            int pid_copy;
            try {
                pid_copy = int.Parse(args[0]);
                if (pid_copy >= pmProcessesList.Count)
                    throw new Exception();
            }
            catch (Exception)  {
                printErrorMsg();
                return;
            }
            try {
                pmProcessesList[pid_copy].Freeze();
            }
            catch (Exception e) {
                Console.WriteLine("[FreezeCommand] Could not freeze. {0}.", e.Message);
            }

        }
    }
}
