using System;
using System.Threading;
using OGPServices;

namespace OGP_PacMan_Server.PuppetMaster {
    public class ServerPuppet : MarshalByRefObject, IProcesses {
        private static readonly object obj = new object();
        private static bool cond;

        public void InjectDelay() {
            throw new NotImplementedException();
        }

        public void Unfreeze() {
            lock (obj) {
                cond = false;
                Monitor.PulseAll(cond);
            }
        }

        void IProcesses.Wait() {
            Wait();
        }

        //Crash and kill should be the same methods?
        public void Crash() {
            Environment.Exit(1);
        }

        public void Freeze() {
            lock (obj) {
                cond = true;
            }
        }

        public void GlobalStatus() {
            throw new NotImplementedException();
        }

        public void LocalStatus() {
            throw new NotImplementedException();
        }


        public static void Wait() {
            lock (obj) {
                while (cond) Monitor.Wait(obj);
            }
        }

        public void Kill() {
            Environment.Exit(1);
        }
    }
}