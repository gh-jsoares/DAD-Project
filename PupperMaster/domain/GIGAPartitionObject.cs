using System;
using System.Collections.Generic;
using System.Linq;

namespace GIGAPuppetMaster.domain
{
    internal class GIGAPartitionObject
    {
        public GIGAPartitionObject(string name, int replicationFactor, string[] serverList, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            Servers = new Dictionary<string, GIGAServerObject>();

            foreach (var serverName in serverList) Servers.Add(serverName, null);

            foreach (var server in servers) Servers[server.Name] = server;
        }

        public string Name { get; }
        public Dictionary<string, GIGAServerObject> Servers { get; }
        public int ReplicationFactor { get; }

        public bool CanSetup()
        {
            return Servers.Values.Where(server => server != null).Count() == ReplicationFactor;
        }

        internal void AddServer(GIGAServerObject serverObject)
        {
            Servers[serverObject.Name] = serverObject;
        }
    }
}