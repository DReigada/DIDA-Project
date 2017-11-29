using System;
using System.Collections.Generic;
using System.Text;

namespace OGPServices {
    public interface IProcessCreationService {
        void CreateProcess();
        void RegisterPM(string url);
    }
}
