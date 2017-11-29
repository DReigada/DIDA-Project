using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster
{
    class Program {

        public static readonly int Port = 11001;
        public static readonly string Name = "PuppetMaster";
        public static readonly string CONFIG_FOLDER_PATH = @"..\..\Resources\Config\";
        public static readonly string CONFIG_FILE_NAME = @"dad-ogp.config";

        static void Main(string[] args) {l channel = new TcpChannel(Port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                 typeof(PuppetMasterService),
                 Name,
                 WellKnownObjectMode.Singleton);*/

            //System.Console.WriteLine("[INIT] PuppetMaster Service is running.");
            //System.Console.WriteLine("[INIT] Press <enter> to exit...");
            // System.Console.ReadLine();

            PuppetMasterShell shell = new PuppetMasterShell();
            shell.start();
        }
    }

    public  class PuppetMasterService : MarshalByRefObject, IPuppetMaster {
        public void sendMsgToPM(string msg) {
            throw new NotImplementedException();
        }
    }
}
