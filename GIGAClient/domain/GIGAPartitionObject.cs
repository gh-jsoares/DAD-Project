using System;
using System.Collections.Generic;
using System.Linq;

namespace GIGAClient.domain
{
    internal class GIGAPartitionObject
    {
        public GIGAPartitionObject(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            if (servers == null) throw new ArgumentNullException(nameof(servers));
            Servers = servers.ToDictionary(server => server.Name, server => server);
        }

        public string Name { get; }
        public Dictionary<string, GIGAServerObject> Servers { get; }
        public int ReplicationFactor { get; }

        internal void ShowStatus()
        {
            Console.WriteLine("\tPartition \"{0}\":", Name);
            foreach (var server in Servers.Values) Console.WriteLine("\t\t{0}", server);
        }

        internal GIGAServerObject GetRandomServer()
        {
            var random = new Random();
            return Servers.Values.ElementAt(random.Next(Servers.Count));
        }

        internal GIGAServerObject GetServer(string serverId)
        {
            return Servers[serverId];
        }

        internal bool HasServer(string serverId)
        {
            return Servers.ContainsKey(serverId);
        }
    }
}