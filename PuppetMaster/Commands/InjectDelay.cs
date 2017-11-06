using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class InjectDelay : Command {
        public InjectDelay(PuppetMasterShell shell) : base(shell, "InjectDelay","InjectDelay src_PID dst_PID") {
        }

        public override void Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
