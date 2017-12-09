using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;

namespace OGPServices {
    public abstract class BaseProcess : MarshalByRefObject, IProcesses {
        private static readonly int DELAY_TIME_MS = 1000;

        private readonly ISet<string> delayedURLs;

        private readonly object freezeLock = new object();
        private bool isFrozen;

        protected BaseProcess() {
            RemotingServices.Marshal(this, "Puppet");
            Console.WriteLine("Initilized BaseProcess");

            delayedURLs = new HashSet<string>();

            ListClientsInfo = () => new List<(int Id, string URL, bool isDead)>();
            ListServersInfo = () => new List<(int Id, string URL, bool isDead)>();
            GetRoundInfo = _ => "";
        }

        public Func<IList<(int Id, string URL, bool isDead)>> ListClientsInfo { get; set; }
        public Func<IList<(int Id, string URL, bool isDead)>> ListServersInfo { get; set; }
        public Func<int, string> GetRoundInfo { get; set; }

        public void GlobalStatus() {
            var clients = string.Join(Environment.NewLine, ListClientsInfo().Select(createString));
            var servers = string.Join(Environment.NewLine, ListServersInfo().Select(createString));

            Console.WriteLine(
                $"GlobalStatus{Environment.NewLine}Clients:{Environment.NewLine}{clients}{Environment.NewLine} Servers:{Environment.NewLine}{servers}");
        }

        public string LocalStatus(int round_id) {
            return GetRoundInfo(round_id);
        }

        public void InjectDelay(string url_dest) {
            lock (delayedURLs) {
                delayedURLs.Add(url_dest);
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

        public void DoDelay(string destURL) {
            if (delayedURLs.Contains(destURL)) Thread.Sleep(DELAY_TIME_MS);
        }

        private string createString((int Id, string URL, bool isDead) tuple) {
            return $"{tuple.Id} {tuple.URL}, {tuple.isDead}";
        }
    }
}