using System;
using System.Threading;

namespace OGP_PacMan_Server.PuppetMaster {
    public static class ServerPuppet {
        private static readonly object obj = new object();
        private static bool cond;


        public static void Wait() {
            lock (obj){
                while (cond) Monitor.Wait(obj);
            }
        }

        public static void Release() {
            lock (obj){
                cond = false;
                Monitor.PulseAll(cond);
            }
        }

        public static void Lock() {
            lock (obj){
                cond = true;
            }
        }

        public static void Kill() {
            Environment.Exit(1);
        }
    }
}