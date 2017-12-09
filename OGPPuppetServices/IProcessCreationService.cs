using System;
using System.Collections.Generic;
using System.Text;

namespace OGPServices {
    public interface IProcessCreationService {
        void createClient(String pid, String clientURL, string masterURL);
        void createServer(String pid, String serverURL, String roundTime, String numPlayers);
        void createClient(String pid, String clientURL, String filename, string masterURL);
    }
}