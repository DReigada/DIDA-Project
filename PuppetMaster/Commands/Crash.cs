using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class Crash : Command {
        public Crash(PuppetMasterShell shell) : base(shell, "Crash","Crash PID") {
        }

        public override void Execute(string[] args) {
            throw new NotImplementedException();
        }
    }
}
