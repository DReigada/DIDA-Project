using System;
using System.Collections.Generic;
using System.Text;

namespace OGPServices {
    public interface IProcessCreationService {
        void createClient(String pid, String clientURL);
        void createServer(String pid, String serverURL, String roundTime, String numPlayers);
        void createClientWithFilename(String pid, String clientURL, String filename);
    }
}