using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using System.Threading;

namespace GIGAServer.services
{
    class GIGAServerService
    {
        public GIGAServerObject Server { get; }
        public int ReplicationFactor { get; set; }
        private bool frozen = false;
        private int minDelay;
        private int maxDelay;

        public GIGAServerService(string id, string hostname, int port, int minDelay, int maxDelay)
        {
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
            Server = new GIGAServerObject(id, string.Format("http://{0}:{1}", hostname, port));
        }

        public string Read(string partitionId, string objectId)
        {
            // TODO
            return null;
        }

        public void Write(string partitionId, GIGAObject value)
        {
            // TODO
        }
        
        public bool Unfreeze()
        {
            frozen = false;
            return true; // success?
        }

        public bool Freeze()
        {
            frozen = true;
            return true; // success?
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
    }
}
