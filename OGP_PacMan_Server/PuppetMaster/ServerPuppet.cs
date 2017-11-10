using System;
using System.Threading;
using OGPServices;

namespace OGP_PacMan_Server.PuppetMaster {
    public class ServerPuppet : MarshalByRefObject, IProcesses {
        private static readonly object obj = new object();
        private static bool cond;


        public static void Wait() {
            lock (obj){
                while (cond) Monitor.Wait(obj);
            }
        }

        public void InjectDelay() {
            throw new NotImplementedException();
        }

        public void Unfreeze() {
            lock (obj){
                cond = false;
                Monitor.PulseAll(cond);
            }
        }

        void IProcesses.Wait() {
            Wait();
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

        public void LocalStatus() {
            throw new NotImplementedException();
        }

        public void Kill() {
            Environment.Exit(1);
        }
    }
}