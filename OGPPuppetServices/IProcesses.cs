using System;
using System.Collections.Generic;
using System.Text;

namespace OGPServices { 
    //Servers & Clients
    public interface IProcesses {
        void Crash();
        void Freeze();
        void GlobalStatus();
        void LocalStatus(int round_id);
        void InjectDelay(string url_dest);
        void Unfreeze();
        void Wait();
    }
}
