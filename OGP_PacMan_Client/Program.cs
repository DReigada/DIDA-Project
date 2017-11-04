using System.Windows.Forms;
using OGPPacManClient.Client;

namespace pacman {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            var clientManager = new ClientManager(16004, 8086);
            clientManager.Start();
        }
    }
}