using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;

namespace GIGAServer.services
{
    class GIGAServerService
    {
        public GIGAServerObject Server { get; }
        public int ReplicationFactor { get; set; }
        public bool Frozen { get; private set; } = false;
        private int minDelay;
        private int maxDelay;

        public Queue<string> FreezeQueue { get; }
        public string PoppedQueue { get; private set; } = "";
        public ManualResetEvent QueuePopEvent { get; } = new ManualResetEvent(false);

        public GIGAServerService(string id, string hostname, int port, int minDelay, int maxDelay)
        {
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
            Server = new GIGAServerObject(id, string.Format("http://{0}:{1}", hostname, port));
            FreezeQueue = new Queue<string>();
        }
        
        public bool Unfreeze()
        {
            Frozen = false;
            PopQueue();
            return true;
        }

        public bool Freeze()
        {
            Frozen = true;
            QueuePopEvent.Reset();
            return true;
        }

        public void ShowStatus()
        {
            Console.WriteLine($"Server {Server.Name} up on URL {Server.Url}");
        }

        public bool Crash()
        {
            // delay in order to respond to puppet master request
            new Timer(delegate { Process.GetCurrentProcess().Kill(); }, null, 2000, Timeout.Infinite);
            return true;
        }

        internal void PopQueue()
        {
            if (FreezeQueue.Count == 0) return;
            PoppedQueue = FreezeQueue.Dequeue();
            QueuePopEvent.Set();
        }
    }
}
