using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GIGAPuppetMaster.domain
{
    class GIGAPartitionObject
    {
        public string Name { get; }
        public Dictionary<string, GIGAServerObject> Servers { get; }
        public int ReplicationFactor { get; }

        public GIGAPartitionObject(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            if (servers == null) throw new ArgumentNullException(nameof(servers));
            Servers = servers.ToDictionary(server => server.Name, server => server);
        }
    }
}
