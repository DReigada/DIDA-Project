using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ClientServerInterface.PacMan.Client.Game;
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
            var form = new Form1();
            var controller = new BoardController(form);

            // TODO: remove this, this is just for testing
            Ghost[] g = {
                new Ghost(GhostColor.Pink, new Position(25, 100), 1),
                new Ghost(GhostColor.Yellow, new Position(100, 66), 2)
            };
            PacManPlayer[] p =
                {new PacManPlayer(3, new Position(200, 299)), new PacManPlayer(4, new Position(150, 150))};

            Coin[] c = {
                new Coin(1, new Position(200, 200)), new Coin(2, new Position(50, 50)),
                new Coin(3, new Position(100, 50))
            };
            var board = new Board(g.ToList(), p.ToList(), c.ToList());


            void TestThread(object o) {
                g[0].Position.X += 1;
                p[0].Position.Y += 1;
                controller.Update(board);
            }


            var timer = new Timer(TestThread, null, 0, 10);

            Application.Run(form);
        }
    }
}