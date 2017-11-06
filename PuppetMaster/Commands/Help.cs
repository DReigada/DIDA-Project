using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    class Help : Command {
        public Help(PuppetMasterShell shell) : base(shell, "Help", "Help") {}

        public override void Execute(string[] args) {
            if (args.Length != 0){
                printErrorMsg(args);
                return;
            }
            Console.WriteLine("Here are the commands available followed by their args:");
            foreach (Command cmd in shell.commands){
                Console.WriteLine(cmd.info);
            }
        }
    }
}
