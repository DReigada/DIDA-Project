using System;
using System.IO;
using System.Windows.Forms;
using OGPPacManClient.Client;

namespace pacman {
    internal static class Program{
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main(string[] args){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length < 3){
                Console.WriteLine("Invalid number of arguments");
                Environment.Exit(1);
            }

            var clientIP = args[0];
            var clientPort = int.Parse(args[1]);
            var serverURL = args[2];


            Console.WriteLine($"Starting client on: {clientIP}:{clientPort} with server: {serverURL}");

            var clientManager = new ClientManager(clientIP, clientPort, serverURL);

            if (args.Length > 3)
                clientManager.UseMovementFile(args[3]);

            clientManager.Start();
        }
    }
}