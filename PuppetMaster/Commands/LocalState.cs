using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class LocalState : Command {
        public LocalState(PuppetMasterShell shell) : base(shell, "LocalState", "LocalState PID round_id") {
        }

        public override void Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
