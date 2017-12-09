using System;
using System.IO;
using OGPServices;

namespace PuppetMaster.commands {
    internal class LocalState : Command {
        public LocalState(PuppetMasterShell shell) : base(shell, "LocalState", "LocalState PID round_id") {
        }

        public override void Execute(string[] args) {
            if (args.Length != 2) {
                printErrorMsg();
                return;
            }
            var pid = args[0];
            var round_id = int.Parse(args[1]);
            IProcesses procProxies;
            var isValid = shell.processes.TryGetValue(pid, out procProxies);
            if (!isValid) {
                Console.WriteLine("[LocalState] There isn't such Process Id: \"{0}\"", pid);
                return;
            }
            try {
                var status = shell.processes[pid].LocalStatus(round_id);
                Console.WriteLine(status);
                WriteToFile(GetFileName(pid, round_id), status);
            }
            catch (Exception e) {
                Console.WriteLine("[LocalState] ERROR: {0}.", e.Message);
            }
        }

        private string GetFileName(string PID, int roundId) {
            return $"LocalState-{PID}-{roundId}";
        }

        private void WriteToFile(string file, string content) {
            File.WriteAllText(file, content);
        }
    }
}