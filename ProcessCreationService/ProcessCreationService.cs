using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using OGPServices;
using OGP_PacMan_Server.Server;

namespace ProcessCreationService {
    public class ProcessCreationService : MarshalByRefObject, IProcessCreationService {

        private Dictionary<String, int> pids;
        private Dictionary<String, IProcesses> processes;
        private List<String> serversURL;
        private List<String> clientsURL;

        public static readonly string SERVER_PATH = @"..\..\..\OGP_PacMan_Server\bin\Debug\OGP_PacMan_Server.exe";
        public static readonly string CLIENT_PATH = @"..\..\..\OGP_PacMan_Client\bin\Debug\OGP_PacMan_Client.exe";

        public ProcessCreationService(){
            pids = new Dictionary<string, int>();
            processes = new Dictionary<string, IProcesses>();
            serversURL = new List<String>();
            clientsURL = new List<String>();
        }

        /*public void RegisterPM(string ip) {

              PuppetMasterChannel = new TcpChannel(, null, null);
              ChannelServices.RegisterChannel(PuppetMasterChannel, false);
              PuppetMaster = (IPuppetMaster)Activator.GetObject(
                  typeof(IPuppetMaster),
                  URL_maker(ip, PM_PORT, PM_NAME));
              Console.WriteLine("[PCS] PuppetMaster Connected");

        }*/

        public static string URL_maker(string ip, int port, string name){
            return "tcp://" + ip + ":" + port + "/" + name;
        }

        public void createClient(string pid, string pcsURL, string clientURL, string roundTime, string numPlayers){
            clientsURL.Add(clientURL);
            try{
                ProcessStartInfo startinfo = new ProcessStartInfo(CLIENT_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Client\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Client.exe";

                //startinfo.Arguments = pid + " " + clientURL + " " + roundTime + " " + numPlayers;
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                Process myProcess = new Process();
                myProcess.StartInfo = startinfo;
                myProcess.Start();
                pids.Add(pid, myProcess.Id);
                Console.WriteLine("Client started at: {0}", clientURL);
            }
            catch (Exception e) {
                Console.WriteLine("[PCS] Could not create the client ERROR: {0}." + e.Message);
            }
        }

        public void createServer(string pid, string pcsURL, string serverURL, string roundTime, string numPlayers){
            serversURL.Add(serverURL);
            String ip = serverURL.Split('/')[2].Split(':')[0];
            String port = serverURL.Split(':')[2].Split('/')[0];

            try
            {
                ProcessStartInfo startinfo = new ProcessStartInfo(SERVER_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Server\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Server.exe";
                startinfo.Arguments = "PacMan " + roundTime + " " + numPlayers + " " + ip + " " + port;
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                Process myProcess = new Process();
                myProcess.StartInfo = startinfo;
                myProcess.Start();
                pids.Add(pid, myProcess.Id);
                Console.WriteLine("Server started at: {0}", serverURL);
            }
            catch (Exception e){
                Console.WriteLine("[PCS] Could not create the server ERROR: {0}." + e.Message);
            }
        }
    }
}
