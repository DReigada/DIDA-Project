using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using OGP_PacMan_Server.Server;

namespace OGP_PacMan_Server {
    internal class Program {
        private static void Main(string[] args) {
            string game;
            int mseconsRound;
            int numberPlayers;
            TcpChannel channel;
            string ip;
            string port;
            string url;
            string masterUrl;
            

            switch (args.Length){
                case 5:
                    game = args[0];
                    mseconsRound = int.Parse(args[1]);
                    numberPlayers = int.Parse(args[2]);
                    ip = args[3];
                    port = args[4];
                    channel = new TcpChannel(int.Parse(port));
                    ChannelServices.RegisterChannel(channel, false);
                    Console.WriteLine(game);
                    Console.WriteLine(mseconsRound);
                    Console.WriteLine(numberPlayers);
                    Console.WriteLine(ip);
                    Console.WriteLine(port);
                    if (game.Equals("Pacman")){
                        url = "tcp://" + ip + ":" + port;
                        var server = new PacManServer(mseconsRound, numberPlayers, url, true);
                        RemotingServices.Marshal(server, "PacManServer", typeof(PacManServer));
                    }
                    break;
                case 6:
                    game = args[0];
                    mseconsRound = int.Parse(args[1]);
                    numberPlayers = int.Parse(args[2]);
                    ip = args[3];
                    port = args[4];
                    channel = new TcpChannel(int.Parse(port));
                    ChannelServices.RegisterChannel(channel, false);
                    masterUrl = args[5];
                    if(game.Equals("Pacman")) {
                        url = "tcp://" + ip + ":" + port;
                        Console.WriteLine(game);
                        Console.WriteLine(mseconsRound);
                        Console.WriteLine(numberPlayers);
                        Console.WriteLine(ip);
                        Console.WriteLine(port);
                        Console.WriteLine(url);
                        Console.WriteLine(masterUrl);
                        var server = new PacManServer(mseconsRound, numberPlayers, url, masterUrl, false);
                        RemotingServices.Marshal(server, "PacManServer", typeof(PacManServer));
                    }
                    break;
            }

            /*var channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);

            Console.WriteLine("What is the game you want to play?");
            var game = Console.ReadLine();

            Console.WriteLine("Enter miliseconds per round");
            var msecondsRound = Console.ReadLine();

            Console.WriteLine("Enter number of players");
            var numberPlayers = Console.ReadLine();

            switch (game){
                case "PacMan":
                    var server = new PacManServer(int.Parse(msecondsRound), int.Parse(numberPlayers));
                    RemotingServices.Marshal(server, "PacManServer", typeof(PacManServer));
                    break;
            }
            */

            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }
    }
}