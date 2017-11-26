using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster.commands {
    abstract class Command {

        public PuppetMasterShell shell {
            get;
        }

        public string cmd {
            get;
            set;
        }

        public string info {
            get;
            set;
        }

        public Command(PuppetMasterShell shell, string cmd, string info){
            this.shell = shell;
            this.cmd = cmd;
            this.info = info;
        }

        abstract public void Execute(string[] args);

        public void printErrorMsg() {
            Console.Write("[ERROR]: In {0} command. For more info type <Help>\n", cmd);
        }
    }
}
