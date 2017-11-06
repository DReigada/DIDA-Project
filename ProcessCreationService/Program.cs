using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCreationService{
    class Program {

        public static readonly int Port = 11000;
        public static readonly string Name = "PCS";

        static void Main(string[] args) {
            var channel = new TcpChannel(Port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ProcessCreationService), 
                Name, 
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("Process Creation Service is running.\nPress <enter> to exit...");
            System.Console.ReadLine();
        }
    }

    public class ProcessCreationService : MarshalByRefObject {
        //TODO create process
    }
}
