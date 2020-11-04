using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GIGAServer.services
{
    class GIGAServerService
    {
        private GIGAServerObject server;
        public int ReplicationFactor { get; set; }
        private bool frozen = false;
        private string hostname;
        private int port;
        private int minDelay;
        private int maxDelay;

        public GIGAServerService(string id, string hostname, int port, int minDelay, int maxDelay)
        {
            this.hostname = hostname;
            this.port = port;
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
            server = new GIGAServerObject(id, string.Format("http://{0}:{1}", hostname, port));
        }

        public GIGAObject Read(string partitionId, string objectId)
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

        public bool ShowStatus()
        {
            // TODO PRINT STATUS INFORMATION TO CONSOLE
            return true;
        }
        public bool Crash()
        {
            // delay in order to respond to puppet master request
            new Timer(delegate { Process.GetCurrentProcess().Kill(); }, null, 2000, Timeout.Infinite);
            return true;
        }
    }
}
