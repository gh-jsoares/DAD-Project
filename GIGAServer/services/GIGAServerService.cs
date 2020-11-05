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
        public bool Frozen { get; private set; } = false;
        private int minDelay;
        private int maxDelay;

        public GIGAServerService(string id, string hostname, int port, int minDelay, int maxDelay)
        {
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
            Server = new GIGAServerObject(id, string.Format("http://{0}:{1}", hostname, port));
        }
        
        public bool Unfreeze()
        {
            Frozen = false;
            return true;
        }

        public bool Freeze()
        {
            Frozen = true;
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
    }
}
