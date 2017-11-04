using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientServerInterface.PacMan.Client.Game;
using OGPPacManClient.Interface;


namespace pacman {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            var controller = new BoardController(form);

            // TODO: remove this, this is just for testing
            Ghost[] g = {new Ghost(GhostColor.Pink, new Position(25, 100),1), new Ghost(GhostColor.Yellow, new Position(100, 66),2)};
            PacManPlayer[] p = {new PacManPlayer(3, new Position(200, 299)), new PacManPlayer(4, new Position(150, 150))};
            var board = new Board(g.ToList(), p.ToList());

            void TestThread() {
                while (true){
                    Thread.Sleep(10);
                    g[0].Position.X += 1;
                    p[0].Position.Y += 1;
                    controller.Update(board);
                }
            }
            new Thread(TestThread).Start();

            Application.Run(form);
        }
    }
}
