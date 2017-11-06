using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands
{
    class Exit : Command {
        public Exit(PuppetMasterShell shell) : base(shell, "Exit", "Exit") {
        }

        public override void Execute(string[] args) {
            throw new NotImplementedException();
        }
    }
}
