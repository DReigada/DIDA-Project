using System;
using System.Threading;
using OGPServices;

namespace OGPPacManClient.PuppetMaster {
    public class ClientPuppet : MarshalByRefObject, IProcesses {
        private static readonly object obj = new object();
        private static bool cond;

        public void Wait() {
            lock (obj){
                while (cond) Monitor.Wait(obj);
            }
        }

        public void InjectDelay(string pid_dest) {
            throw new NotImplementedException();
        }

        public void Unfreeze() {
            lock (obj){
                cond = false;
                Monitor.PulseAll(cond);
            }
        }

        public void Crash() {
            throw new NotImplementedException();
        }

        public void Freeze() {
            lock (obj){
                cond = true;
            }
        }

        public void GlobalStatus() {
            throw new NotImplementedException();
        }

        public void LocalStatus(int round_id) {
            throw new NotImplementedException();
        }

        public void Kill() {
            Environment.Exit(1);
        }
    }
}