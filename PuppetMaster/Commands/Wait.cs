using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster.commands {
    class Wait : Command{
        public Wait(PuppetMasterShell shell) : base(shell, "Wait", "Wait x_ms") {
        }

        public override void Execute(string[] args) {
            if (args.Length != 1) {
                printErrorMsg();
                return;
            }
            int x_ms = int.Parse(args[0]);
            try{
                Console.WriteLine("Waiting... {0} ms", x_ms);
                System.Threading.Thread.Sleep(x_ms);
            }
            catch (Exception e){
                Console.WriteLine("[Wait] ERROR: {0}.", e.Message);
            }
        }
    }
}
