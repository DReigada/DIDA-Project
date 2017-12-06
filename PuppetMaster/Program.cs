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

        static void Main(string[] args) {
            PuppetMasterShell shell = new PuppetMasterShell();
            shell.start();
        }
    }
}
