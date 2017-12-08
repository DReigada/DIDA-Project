using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ClientServerInterface.PacMan.Server;

namespace OGPPacManClient.Client.Movement {
    public class FileMovementController : AbstractMovementController {
        private readonly Regex lineRegex = new Regex(@"\d*,\s?(\S*)");
        private readonly Queue<string> linesQueue;

        public FileMovementController(string file, IPacmanServer server, string serverUrl, int delta, int userId) : base(server, serverUrl, delta,
            userId) {
            var lines = File.ReadAllLines(file);

            linesQueue = new Queue<string>(lines);
        }

        public override ClientServerInterface.PacMan.Server.Movement.Direction GetDirection() {
            if (linesQueue.Count != 0) {
                var line = linesQueue.Dequeue();
                var parsedLine = ParseLine(line);
                switch (parsedLine) {
                    case "UP":
                        return ClientServerInterface.PacMan.Server.Movement.Direction.Up;
                    case "DOWN":
                        return ClientServerInterface.PacMan.Server.Movement.Direction.Down;
                    case "LEFT":
                        return ClientServerInterface.PacMan.Server.Movement.Direction.Left;
                    case "RIGHT":
                        return ClientServerInterface.PacMan.Server.Movement.Direction.Right;
                    default:
                        return ClientServerInterface.PacMan.Server.Movement.Direction.Stopped;
                }
            }
            return ClientServerInterface.PacMan.Server.Movement.Direction.Stopped;
        }

        public bool HasNext() {
            return linesQueue.Count > 0;
        }

        private string ParseLine(string line) {
            var m = lineRegex.Match(line);
            return m.Groups[1].Captures[0].Value;
        }
    }
}