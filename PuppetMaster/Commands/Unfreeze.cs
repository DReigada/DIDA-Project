using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class Unfreeze : Command {
        public Unfreeze(PuppetMasterShell shell) : base(shell, "Unfreeze", "Unfreeze PID") {
        }

        public override void Execute(string[] args) {
            throw new NotImplementedException();
        }
    }
}
