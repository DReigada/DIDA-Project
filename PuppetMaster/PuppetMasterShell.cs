﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppetMaster.commands;
using System.Threading;
using OGPServices;
using System.Runtime.Remoting.Channels.Tcp;

namespace PuppetMaster {
    class PuppetMasterShell : MarshalByRefObject {
        private string prompt = "[PuppetMaster] >>> ";
        
        public Dictionary<string, List<IProcesses>> processesProxies { get; }

        public List<Command> commands {
            get;
        }

        public PuppetMasterShell() {
            commands = new List<Command>();

            processesProxies = new Dictionary<string, List<IProcesses>>();

            //List<IProcesses> pProxies = new List<IProcesses>();
            //string URL = $"tcp://localhost:{8086}/PuppetMaster";
            //IProcesses remoteProcesses = (IProcesses)Activator.GetObject(typeof(IProcesses), URL);
            //pProxies.Add(remoteProcesses);
            //Console.WriteLine("[PuppetMaster] Proxu created at {0}", URL);



            commands.Add(new Crash(this));
            commands.Add(new Freeze(this));
            commands.Add(new GlobalStatus(this));
            commands.Add(new Help(this));
            commands.Add(new InjectDelay(this));
            commands.Add(new LocalState(this));
            commands.Add(new StartClient(this));
            commands.Add(new StartServer(this));
            commands.Add(new Unfreeze(this));
            commands.Add(new Wait(this));
            commands.Add(new Exit(this));

        }


        public Command getCommand(string input){
            foreach (Command cmd in commands) {
                if (string.Equals(cmd.cmd, input, StringComparison.OrdinalIgnoreCase))
                    return cmd;
            }
            return null;
        }

        public void doCommand(string input) {
            string[] args = input.Split(' ');
            string cmd = args[0];

            Command command = getCommand(cmd);
            if (command == null){
                Console.WriteLine("[PuppetMaster] Invalid command <{0}>.", cmd);
            }
            else{
                string[] cmdArgs = args.Skip(1).ToArray(); //only command args 
                command.Execute(cmdArgs);
            }
        }

        public void start() {
            Console.WriteLine("[PuppetMaster] Starting PuppetMaster...");
            Console.WriteLine("[PuppetMaster] PuppetMaster started. Type <help> for more info");
            while (true) {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (input == null) {
                    continue;
                }
                doCommand(input);
            }
        }

    }
}
