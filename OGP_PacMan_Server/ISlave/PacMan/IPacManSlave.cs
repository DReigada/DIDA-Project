using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServerInterface.PacMan.Server;

namespace OGP_PacMan_Server.ISlave.PacMan
{
    public interface IPacManSlave: IPacmanServer, ISlave<Movement, GameState> {
    }
}
