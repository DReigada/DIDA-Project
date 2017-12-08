using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppetMaster.commands;
using System.Threading;
using OGPServices;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting;

namespace PuppetMaster {
    class PuppetMasterShell{

        private TextReader stdin { get; set; }
        public bool readingFile { get; set; }
        
        public string prompt = "[PuppetMaster] >>> ";

        public Dictionary<string, String> processesURLs = new Dictionary<string, String>();
        public Dictionary<string, IProcesses> processes = new Dictionary<string, IProcesses>();

        public IProcessCreationService pcs = null;

        public List<Command> commands {
            get;
        }

        public PuppetMasterShell() {
            commands = new List<Command>();
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
            
            parseConfigFile();

        }
        public IProcessCreationService connectPCS(string url){
            try{
                pcs = (IProcessCreationService)Activator.GetObject(typeof(IProcessCreationService), url);
            }
            catch (Exception) {
                Console.WriteLine("Couldn't reach IProcessCreationService.");
            }
            return pcs;
        }

        public Command getCommand(string input){
            foreach (Command cmd in commands) {
                if (string.Equals(cmd.cmd, input, StringComparison.OrdinalIgnoreCase))
                    return cmd;
            }
            return null;
        }

        public void doCommandAsync(string input) {
            string[] args = input.Split(' ');
            string cmd = args[0];

            Command command = getCommand(cmd);
            if (command == null){
                Console.WriteLine("[PuppetMaster] Invalid command <{0}>.", cmd);
            }
            else {
                string[] cmdArgs = args.Skip(1).ToArray(); //only command args 
                //await Task.Delay(5000);
                new Thread(() => command.Execute(cmdArgs)).Start();
            }
        }

        public void start() {
            Console.WriteLine("[PuppetMaster] Type <help> for more info");
            while (true) {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (input == null) {
                    continue;
                }

                string[] args = input.Split(' ');
                string cmd = args[0];
                string[] cmdArgs = args.Skip(1).ToArray();
                if (string.Equals(cmd, "wait", StringComparison.OrdinalIgnoreCase)){
                    doWait(cmdArgs);
                }

                doCommandAsync(input);
            }
        }

        public void doWait(string[] x_ms) {
            Command command = getCommand("Wait");
            command.Execute(x_ms);
        }

        private void parseConfigFile(){
            TextReader fileInput;
            string configFilePath;
            
            Console.WriteLine("[CONFIGURATION] What's the name of the config file? (Press <enter> for default config file \"{0}\")", Program.CONFIG_FILE_NAME);
            Console.Write("[CONFIGURATION] FileName: ");
            string inputConfigFile = Console.ReadLine();
            if (inputConfigFile == "")
                configFilePath = Program.CONFIG_FOLDER_PATH + Program.CONFIG_FILE_NAME;
            else
                configFilePath = Program.CONFIG_FOLDER_PATH + inputConfigFile;

            try{
                fileInput = File.OpenText(configFilePath);
                Console.WriteLine("[CONFIGURATION] Open config file at: {0}.", configFilePath);
            }
            catch (Exception){
                Console.WriteLine("[CONFIGURATION] Errrr... :/ Could not reach the file: {0}.", configFilePath);
                return;
            }

            Console.WriteLine("[CONFIGURATION] Do you want to run the config file \"{0}\" step-by-step?", Program.CONFIG_FILE_NAME);
            Console.Write("[CONFIGURATION] Y/N: ");
            string input = Console.ReadLine();
            bool step_by_step = false;
            if (string.Equals("y", input, StringComparison.OrdinalIgnoreCase) || string.Equals("yes", input, StringComparison.OrdinalIgnoreCase))
                step_by_step = true;
            string inputLine;

            while ((inputLine = fileInput.ReadLine()) != null){
                inputLine = inputLine.TrimEnd(' ');
                string[] args = inputLine.Split(' ');
                string command = args[0];

                if (step_by_step){
                    Console.Write("{0} \"{1}\"", prompt, inputLine);
                    Console.ReadLine();
                }
                else{
                    Console.WriteLine("{0} \"{1}\"", prompt, inputLine);
                }

                if (string.Equals("wait", command, StringComparison.OrdinalIgnoreCase)){
                    string[] cmdArgs = args.Skip(1).ToArray();
                    doWait(cmdArgs);
                }

                System.Threading.Thread.Sleep(50);
                doCommandAsync(inputLine);
            }
            Console.WriteLine("[CONFIGURATION] END!");
        }
    }
}
