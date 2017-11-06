using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class GlobalStatus : Command{
        public GlobalStatus(PuppetMasterShell shell) : base(shell, "GlobalStatus","GlobalStatus") {
        }

        public override void Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}

