using System;
using System.Threading;
using OGPServices;

namespace OGPPacManClient.PuppetMaster {
    public class ClientPuppet : MarshalByRefObject, IProcesses {
        private static readonly object freezeLock = new object();
        private static bool isFrozen;

        private static ClientPuppet _instance;

        private ClientPuppet() {
        }

        public static ClientPuppet Instance => _instance ?? (_instance = new ClientPuppet());

        public void Wait() {
            lock (freezeLock) {
                while (isFrozen) Monitor.Wait(freezeLock);
            }
        }

        public void InjectDelay(string pid_dest) {
            throw new NotImplementedException();
        }

        public void Unfreeze() {
            lock (freezeLock) {
                isFrozen = false;
                Monitor.PulseAll(isFrozen);
            }
        }

        public void Crash() {
            Environment.Exit(1);
        }

        public void Freeze() {
            lock (freezeLock) {
                isFrozen = true;
            }
        }

        public void GlobalStatus() {
            throw new NotImplementedException();
        }

        public void LocalStatus(int round_id) {
            throw new NotImplementedException();
        }
    }
}