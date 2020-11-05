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

        public GIGAPartitionObject(string name, int replicationFactor, string[] serverList, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            Servers = new Dictionary<string, GIGAServerObject>();

            foreach(string serverName in serverList)
            {
                Servers.Add(serverName, null);
            }

            foreach(GIGAServerObject server in servers)
            {
                Servers[server.Name] = server;
            }
        }

        public bool CanSetup() => Servers.Values.Where(server => server != null).Count() == ReplicationFactor;

        internal void AddServer(GIGAServerObject serverObject)
        {
            Servers[serverObject.Name] = serverObject;
        }
    }
}
