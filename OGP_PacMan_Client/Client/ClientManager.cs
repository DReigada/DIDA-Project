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
        private readonly string clientIP;
        private readonly int clientPort;
        public readonly BoardController controller;
        private readonly Form1 form;
        private readonly string serverURL;
        private ChatController chatController;
        private AbstractMovementController moveController;
        private string movementFile;
        private IPacmanServer server;

        public ClientManager(string clientIP, int clientPort, string serverURL) {
            this.clientIP = clientIP;
            this.clientPort = clientPort;
            this.serverURL = serverURL;

            form = new Form1();
            controller = new BoardController(form);


            client = new ClientImpl(controller, UpdateServer);
        }

        public void Start() {
            new Thread(() => Application.Run(form)).Start();

            RegisterTCPChannel();
            RegisterClientChannel();
            server = GetServerConnection();

            var clientInfo = new ClientInfo($"tcp://{clientIP}:{clientPort}");
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
            var channel = new TcpChannel(clientPort);
            ChannelServices.RegisterChannel(channel, false);
        }


        private void RegisterClientChannel() {
            RemotingServices.Marshal(client, "PacManClient");
        }

        private IPacmanServer GetServerConnection() {
            return (IPacmanServer)
                Activator.GetObject(
                    typeof(IPacmanServer),
                    $"tcp://{serverURL}/PacManServer");
        }


        private void UpdateServer(string url) {
            Console.WriteLine(url);
            server = (IPacmanServer) Activator.GetObject(typeof(IPacmanServer), url + "/PacManServer");
            moveController.setNewServer(server);
        }
    }
}