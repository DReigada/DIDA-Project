using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class Freeze : Command  {
        public Freeze(PuppetMasterShell shell) : base(shell, "Freeze", "Freeze PID") {
        }


        public override void Execute(string[] args) {
            
        }
    }
}
