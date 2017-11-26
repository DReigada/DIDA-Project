using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class Wait : Command{
        public Wait(PuppetMasterShell shell) : base(shell, "Wait", "Wait x_ms") {
        }

        public override void Execute(string[] args) {
            throw new NotImplementedException();
        }
    }
}
