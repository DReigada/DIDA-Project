using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace PuppetMaster
{
    class Program {

        public static readonly int Port = 11001;
        public static readonly string Name = "PuppetMaster";

        static void Main(string[] args) {

            TcpChannel channel = new TcpChannel(Port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                 typeof(PuppetMasterService),
                 Name,
                 WellKnownObjectMode.Singleton);

            System.Console.WriteLine("PuppetMaster Service is running.\nPress <enter> to exit...");
            System.Console.ReadLine();

           

            PuppetMasterShell shell = new PuppetMasterShell();
            //RemotingServices.Marshal(shell, Name);
           
                    
            
            shell.start();
        }
    }

    public  class PuppetMasterService : MarshalByRefObject, IPuppetMaster {
    }
}
