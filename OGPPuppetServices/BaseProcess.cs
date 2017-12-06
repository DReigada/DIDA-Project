using System;
using System.Threading;

namespace OGPServices {
    public abstract class BaseProcess : MarshalByRefObject, IProcesses {
        private readonly object freezeLock = new object();
        private bool isFrozen;


        public abstract void GlobalStatus();

        public abstract void LocalStatus(int round_id);

        public abstract void InjectDelay(string pid_dest);

        public void Crash() {
            Environment.Exit(1);
        }

        public void Freeze() {
            lock (freezeLock) {
                isFrozen = true;
            }
        }

        public void Unfreeze() {
            lock (freezeLock) {
                isFrozen = false;
                Monitor.PulseAll(isFrozen);
            }
        }

        public void Wait() {
            lock (freezeLock) {
                while (isFrozen) Monitor.Wait(freezeLock);
            }
        }
    }
}