using System;
using System.Collections.Generic;
using System.Text;

namespace OGPServices {
    public interface IProcessCreationService {
        void createClient(String pid, String pcsURL, String clientURL, String roundTime, String numPlayers);
        void createServer(String pid, String pcsURL, String serverURL, String roundTime, String numPlayers);
    }
}