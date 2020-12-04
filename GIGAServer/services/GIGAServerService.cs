using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GIGAServer.domain;

namespace GIGAServer.services
{
    internal class GIGAServerService
    {
        public GIGAServerService(string id, string hostname, int port, int minDelay, int maxDelay)
        {
            MinDelay = minDelay;
            MaxDelay = maxDelay;
            Server = new GIGAServerObject(id, string.Format("http://{0}:{1}", hostname, port));
        }

        public GIGAServerObject Server { get; }
        public int ReplicationFactor { get; set; }
        public bool Frozen { get; private set; }
        public bool IsWriting { get; set; } = false;
        public int MinDelay { get; }
        public int MaxDelay { get; }

        public Queue<string> FreezeQueue { get; } = new Queue<string>();
        public string FreezePoppedQueue { get; private set; } = "";
        public ManualResetEvent FreezeQueuePopEvent { get; } = new ManualResetEvent(false);
        public Queue<string> FreezeQueueHeartbeat { get; } = new Queue<string>();
        public string FreezePoppedQueueHeartbeat { get; private set; } = "";
        public ManualResetEvent FreezeQueueHeartbeatPopEvent { get; } = new ManualResetEvent(false);

        public Queue<string> WriteQueue { get; } = new Queue<string>();
        public string WritePoppedQueue { get; private set; } = "";
        public ManualResetEvent WriteQueuePopEvent { get; } = new ManualResetEvent(false);

        public bool Unfreeze()
        {
            Frozen = false;
            PopFreezeQueue();
            while (FreezeQueueHeartbeat.Count != 0)
                PopFreezeQueueHeartbeat();
            return true;
        }

        public bool Freeze()
        {
            Frozen = true;
            FreezeQueuePopEvent.Reset();
            FreezeQueueHeartbeatPopEvent.Reset();
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

        internal void PopFreezeQueue()
        {
            if (FreezeQueue.Count == 0) return;
            FreezePoppedQueue = FreezeQueue.Dequeue();
            FreezeQueuePopEvent.Set();
        }

        internal void PopFreezeQueueHeartbeat()
        {
            if (FreezeQueueHeartbeat.Count == 0) return;
            FreezePoppedQueueHeartbeat = FreezeQueueHeartbeat.Dequeue();
            FreezeQueueHeartbeatPopEvent.Set();
        }

        internal void PopWriteQueue()
        {
            if (WriteQueue.Count == 0) return;
            WritePoppedQueue = WriteQueue.Dequeue();
            WriteQueuePopEvent.Set();
        }
    }
}