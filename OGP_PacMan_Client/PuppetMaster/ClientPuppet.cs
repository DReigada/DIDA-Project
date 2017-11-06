using System.Threading;

namespace OGPPacManClient.PuppetMaster {
    public static class ClientPuppet {
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
    }
}