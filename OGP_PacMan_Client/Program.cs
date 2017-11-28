using System;
using System.Windows.Forms;
using OGPPacManClient.Client;

namespace pacman {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var port = new Random().Next(16000, 16210);
            Console.WriteLine($"Using port: {port}");
            var clientManager = new ClientManager(port, 8086);
            if (args.Length > 0){
                clientManager.UseMovementFile(args[0]);
            }
            clientManager.Start();
        }
    }
}