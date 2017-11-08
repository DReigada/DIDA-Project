using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using OGP_PacMan_Server.Server;

namespace OGP_PacMan_Server {
    internal class Program {
        private static void Main(string[] args) {
            var channel = new TcpChannel(8086);
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


            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }
    }
}