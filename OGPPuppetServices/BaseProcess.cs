﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;

namespace OGPServices {
    public abstract class BaseProcess : MarshalByRefObject, IProcesses {
        private static readonly int DELAY_TIME_MS = 3000;

        private readonly object freezeLock = new object();
        private bool isFrozen;

        private ISet<String> delayedURLs;

        protected BaseProcess() {
            RemotingServices.Marshal(this, "Puppet");
            Console.WriteLine("Initilized BaseProcess");

            delayedURLs = new HashSet<string>();

            ListClientsInfo = () => new List<(int Id, string URL, bool isDead)>();
            ListServersInfo = () => new List<(int Id, string URL, bool isDead)>();
        }

        public Func<IList<(int Id, string URL, bool isDead)>> ListClientsInfo { get; set; }
        public Func<IList<(int Id, string URL, bool isDead)>> ListServersInfo { get; set; }

        public void GlobalStatus() {
            var clients = string.Join("\n", ListClientsInfo().Select(createString));
            var servers = string.Join("\n", ListServersInfo().Select(createString));

            Console.WriteLine($"GlobalStatus\nClients:\n{clients}\n Servers:\n{servers}");
        }

        public abstract void LocalStatus(int round_id);

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

        public void DoDelay(String destURL) {
            if (delayedURLs.Contains(destURL)) {
                Thread.Sleep(DELAY_TIME_MS);
            }
        }

        private string createString((int Id, string URL, bool isDead) tuple) {
            return $"{tuple.Id} {tuple.URL}, {tuple.isDead}";
        }
    }
}