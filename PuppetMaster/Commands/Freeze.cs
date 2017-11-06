using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class Freeze : Command {
        public Freeze(PuppetMasterShell shell) : base(shell, "Freeze", "Freeze PID") {
        }

        public override void Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
