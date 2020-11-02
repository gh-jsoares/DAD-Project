using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAServer.domain
{
    class GIGAPartitionObject
    {
        public string Name { get; }
        private Dictionary<string, GIGAServerObject> servers;
        public int ReplicationFactor { get; }
        public GIGAServerObject MasterServer {
            get
            {
                // if server is down, change server
                return MasterServer;
            }
        }
        private Dictionary<string, GIGAObject> objects;

        public GIGAPartitionObject(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            if (servers == null) throw new ArgumentNullException(nameof(servers));
            this.servers = new Dictionary<string, GIGAServerObject>();
            this.objects = new Dictionary<string, GIGAObject>();

            foreach (GIGAServerObject server in servers)
                this.servers.Add(server.Name, server);
        }

        public GIGAObject GetObject(string name)
        {
            return objects.GetValueOrDefault(name, null);
        }

        public bool HasObject(string name)
        {
            return objects.ContainsKey(name);
        }

        public void AddObject(string name, GIGAObject value)
        {
            objects.Add(name, value);
        }
    }
}
