using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
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

        public static string URL_maker(string ip, int port, string name){
            return "tcp://" + ip + ":" + port + "/" + name;
        }

        public static string url_with_port_ip(string ip, int port)
        {
            return ip + ":" + port;
        }

        public void createClient(string pid, string clientURL){
            String clientIp = clientURL.Split('/')[2].Split(':')[0];
            String clientPort = clientURL.Split(':')[2].Split('/')[0];
            clientsURL.Add(url_with_port_ip(clientIp, int.Parse(clientPort)));

            try {
                ProcessStartInfo startinfo = new ProcessStartInfo(CLIENT_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Client\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Client.exe";
                startinfo.Arguments = clientIp + " " + clientPort + " " + serversURL[0];
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

    
        public void createClient(string pid, string clientURL, string filename)
        {
            String clientIp = clientURL.Split('/')[2].Split(':')[0];
            String clientPort = clientURL.Split(':')[2].Split('/')[0];
            clientsURL.Add(url_with_port_ip(clientIp, int.Parse(clientPort)));

            try
            {
                ProcessStartInfo startinfo = new ProcessStartInfo(CLIENT_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Client\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Client.exe";
                startinfo.Arguments = clientIp + " " + clientPort + " " + serversURL[0] + filename;
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                Process myProcess = new Process();
                myProcess.StartInfo = startinfo;
                myProcess.Start();
                pids.Add(pid, myProcess.Id);
                Console.WriteLine("Client started at: {0}", clientURL);
            }
            catch (Exception e)
            {
                Console.WriteLine("[PCS] Could not create the client ERROR: {0}." + e.Message);
            }
        }

        public void createServer(string pid, string serverURL, string roundTime, string numPlayers){
            
            String serverIp = serverURL.Split('/')[2].Split(':')[0];
            String serverPort = serverURL.Split(':')[2].Split('/')[0];
            serversURL.Add(url_with_port_ip(serverIp, int.Parse(serverPort)));
            try{
                ProcessStartInfo startinfo = new ProcessStartInfo(SERVER_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Server\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Server.exe";
                startinfo.Arguments = "Pacman " + roundTime + " " + numPlayers + " " + serverIp + " " + serverPort;
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
