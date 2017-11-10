using System;
using System.Collections.Generic;
using System.Text;

namespace OGPServices { 
    //Servers & Clients
    public interface IProcesses {
        void Crash();
        void Freeze();
        void GlobalStatus();
        void LocalStatus();
        void InjectDelay();
        void Unfreeze();
        void Wait();
    }
}
