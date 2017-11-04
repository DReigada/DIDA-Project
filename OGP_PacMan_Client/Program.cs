using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ClientServerInterface.PacMan.Client.Game;
using OGPPacManClient.Client;
using OGPPacManClient.Interface;
using Timer = System.Threading.Timer;

namespace pacman {
    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            var clientManager = new ClientManager();
            clientManager.Start();
        }
    }
}