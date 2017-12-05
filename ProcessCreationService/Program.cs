using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using OGPServices;

namespace ProcessCreationService {

    class Program {

        public static readonly int PCS_PORT = 11000;
        public static readonly string PCS_NAME = "PCS";

        static void Main(string[] args) { 

            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = PCS_PORT;
            TcpChannel channel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(channel, false);

            ProcessCreationService pcs = new ProcessCreationService();
            RemotingServices.Marshal(pcs, PCS_NAME, typeof(ProcessCreationService));


            Console.WriteLine("Process Creation Service is running.\nPress <enter> to exit...");
            Console.ReadLine();
        }
    }
}
