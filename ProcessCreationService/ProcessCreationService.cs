using System;
using System.Collections.Generic;
using System.Diagnostics;
using OGPServices;

namespace ProcessCreationService {
    public class ProcessCreationService : MarshalByRefObject, IProcessCreationService {
        public static readonly string SERVER_PATH = @"..\..\..\OGP_PacMan_Server\bin\Debug\OGP_PacMan_Server.exe";
        public static readonly string CLIENT_PATH = @"..\..\..\OGP_PacMan_Client\bin\Debug\OGP_PacMan_Client.exe";
        private readonly List<string> clientsURL;

        private readonly Dictionary<string, int> pids;
        private Dictionary<string, IProcesses> processes;

        public ProcessCreationService() {
            pids = new Dictionary<string, int>();
            processes = new Dictionary<string, IProcesses>();
            clientsURL = new List<string>();
        }

        public void createClient(string pid, string clientURL, string masterURL) {
            var clientIp = clientURL.Split('/')[2].Split(':')[0];
            var clientPort = clientURL.Split(':')[2].Split('/')[0];
            clientsURL.Add(url_with_port_ip(clientIp, int.Parse(clientPort)));

            try {
                var startinfo = new ProcessStartInfo(CLIENT_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Client\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Client.exe";
                startinfo.Arguments = clientIp + " " + clientPort + " " + masterURL;
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                var myProcess = new Process();
                myProcess.StartInfo = startinfo;
                myProcess.Start();
                pids.Add(pid, myProcess.Id);
                Console.WriteLine("Client started at: {0}", clientURL);
            }
            catch (Exception e) {
                Console.WriteLine("[PCS] Could not create the client ERROR: {0}." + e.Message);
            }
        }


        public void createClient(string pid, string clientURL, string filename, string masterURL) {
            var clientIp = clientURL.Split('/')[2].Split(':')[0];
            var clientPort = clientURL.Split(':')[2].Split('/')[0];
            clientsURL.Add(url_with_port_ip(clientIp, int.Parse(clientPort)));

            try {
                var startinfo = new ProcessStartInfo(CLIENT_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Client\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Client.exe";
                startinfo.Arguments = clientIp + " " + clientPort + " " + masterURL + filename;
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                var myProcess = new Process();
                myProcess.StartInfo = startinfo;
                myProcess.Start();
                pids.Add(pid, myProcess.Id);
                Console.WriteLine("Client started at: {0}", clientURL);
            }
            catch (Exception e) {
                Console.WriteLine("[PCS] Could not create the client ERROR: {0}." + e.Message);
            }
        }

        public void createServer(string pid, string serverURL, string roundTime, string numPlayers) {
            var serverIp = serverURL.Split('/')[2].Split(':')[0];
            var serverPort = serverURL.Split(':')[2].Split('/')[0];

            try {
                var startinfo = new ProcessStartInfo(SERVER_PATH);
                startinfo.UseShellExecute = true;
                startinfo.WorkingDirectory = @"..\..\..\OGP_PacMan_Server\bin\Debug\";
                startinfo.FileName = @"OGP_PacMan_Server.exe";
                startinfo.Arguments = "Pacman " + roundTime + " " + numPlayers + " " + serverIp + " " + serverPort;
                startinfo.WindowStyle = ProcessWindowStyle.Normal;
                var myProcess = new Process();
                myProcess.StartInfo = startinfo;
                myProcess.Start();
                pids.Add(pid, myProcess.Id);
                Console.WriteLine("Server started at: {0}", serverURL);
            }
            catch (Exception e) {
                Console.WriteLine("[PCS] Could not create the server ERROR: {0}." + e.Message);
            }
        }

        public static string URL_maker(string ip, int port, string name) {
            return "tcp://" + ip + ":" + port + "/" + name;
        }

        public static string url_with_port_ip(string ip, int port) {
            return ip + ":" + port;
        }
    }
}