using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace ProcessCreationService {

    class Program {

        public static readonly int PCS_PORT = 11000;
        public static readonly string PCS_NAME = "ProcessCreationService";
        public static readonly int PM_PORT = 11001;
        public static readonly string PM_NAME = "PuppetMaster";

        static void Main(string[] args) {
            TcpChannel channel = new TcpChannel(PCS_PORT);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ProcessCreationService),
                PCS_NAME,
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("Process Creation Service is running.\nPress <enter> to exit...");
            System.Console.ReadLine();

        }

        public class ProcessCreationService : MarshalByRefObject, IProcessCreationService {
            private IPuppetMaster PuppetMaster;
            private TcpChannel PuppetMasterChannel;

            public void CreateProcess() {
                /*try {
                    ProcessStartInfo startinfo = new ProcessStartInfo();
                    startinfo.UseShellExecute = true;
                    startinfo.WorkingDirectory = @"..\..\..\";
                    startinfo.FileName = @"bin\Debug\Process.exe";
    
                    startinfo.Arguments = pid + " " + pcs_url + " " + client_url + " " + msec_per_round + " " + num_player;
                    startinfo.WindowStyle = ProcessWindowStyle.Normal;
                    Process myProcess = new Process();
                    myProcess.StartInfo = startinfo;
                    myProcess.Start();
                }
                catch (Exception e) {
                    Console.WriteLine("[PCS] Could not create the process. Caused by: {0}." + e.Message);
                }*/
            }

            public void RegisterPM(string ip) {
                /*
                 PuppetMasterChannel = new TcpChannel(PUT, null, null);
                 ChannelServices.RegisterChannel(PuppetMasterChannel, false);
                 PuppetMaster = (IPuppetMaster)Activator.GetObject(
                     typeof(IPuppetMaster),
                     URL_maker(ip, PM_PORT, PM_NAME));*/

            }

            public static string URL_maker(string ip, int port, string name) {
                return "tcp://" + ip + ":" + port + "/" + name;
            }
        }
    }
}
