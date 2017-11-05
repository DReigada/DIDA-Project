using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace OGP_PacMan_Server {
    internal class Program {
        private static void Main(string[] args) {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);

            Console.WriteLine("Enter miliseconds per round");
            var msecondsRound = Console.ReadLine();

            Console.WriteLine("Enter number of players");
            var numberPlayers = Console.ReadLine();

            PacManServer server = new PacManServer(int.Parse(msecondsRound), int.Parse(numberPlayers));

            RemotingServices.Marshal(server, "PacManServer", typeof(PacManServer));

            Console.WriteLine("Press <enter> to exit");
            Console.ReadLine();
        }
        
    }
}