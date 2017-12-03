using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Windows.Forms;
using ClientServerInterface.PacMan.Server;
using ClientServerInterface.Server;
using OGPPacManClient.Client.Chat;
using OGPPacManClient.Client.Movement;
using OGPPacManClient.Interface;

namespace OGPPacManClient.Client {
    internal class ClientManager {
        private readonly ClientImpl client;
        public readonly BoardController controller;
        private readonly Form1 form;
        private readonly int port;
        private readonly int serverPort;
        private ChatController chatController;
        private AbstractMovementController moveController;
        private string movementFile;
        private IPacmanServer server;

        public ClientManager(int port, int serverPort) {
            this.port = port;
            this.serverPort = serverPort;

            form = new Form1();
            controller = new BoardController(form);

           
            client = new ClientImpl(controller, UpdateServer);
        }

        public void Start() {
            new Thread(() => Application.Run(form)).Start();

            RegisterTCPChannel();
            RegisterClientChannel();
            server = GetServerConnection();

            var clientInfo = new ClientInfo($"tcp://localhost:{port}"); //TODO what is name?
            var gameProps = server.RegisterClient(clientInfo);
            InitializeControllers(gameProps);
        }

        public void UseMovementFile(string file) {
            movementFile = file;
        }

        private void InitializeControllers(GameProps gameProps) {
            if (movementFile != null)
                moveController =
                    new FileMovementController(movementFile, server, gameProps.GameSpeed, gameProps.UserId);
            else
                moveController = new MovementController(form, server, gameProps.GameSpeed, gameProps.UserId);

            moveController.Start();

            chatController = new ChatController(gameProps.UserId);
            client.NewConnectedClients += chatController.AddClients;
            chatController.AddClients(client.ConnectedClients);
            form.NewMessage += chatController.SendMessage;
            chatController.IncomingMessage += form.AddMessage;
        }

        private void RegisterTCPChannel() {
            var channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
        }


        private void RegisterClientChannel() {
            RemotingServices.Marshal(client, "PacManClient");
        }

        private IPacmanServer GetServerConnection() {
            return (IPacmanServer)
                Activator.GetObject(
                    typeof(IPacmanServer),
                    $"tcp://localhost:{serverPort}/PacManServer");
        }
        

        private void UpdateServer(String url) {
            Console.WriteLine(url);
            server = (IPacmanServer) Activator.GetObject( typeof(IPacmanServer), url + "/PacManServer");
            moveController.setNewServer(server);
        }

        // TODO: remove this, this is just for testing
        /*  public void Test() {
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


            var timer = new Timer(TestThread, null, 100, 10);
        }*/
    }
}