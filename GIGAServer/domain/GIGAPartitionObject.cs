using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GIGAServer.domain
{
    class GIGAPartitionObject
    {
        public string Name { get; }
        private Dictionary<string, GIGAServerObject> servers;
        public int ReplicationFactor { get; }
        public GIGAServerObject MasterServer { get; }
        private Dictionary<string, GIGAObject> objects;

        public GIGAPartitionObject(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            if (servers == null) throw new ArgumentNullException(nameof(servers));
            this.servers = servers.ToDictionary(server => server.Name, server => server);
            this.objects = new Dictionary<string, GIGAObject>();
            this.MasterServer = servers.First();
        }

        internal void ShowStatus()
        {
            Console.WriteLine("Partition \"{0}\":\n\tCurrent Master: {1}", Name, MasterServer.ToString());
            foreach (KeyValuePair<string, GIGAServerObject> entry in servers)
            {
                Console.WriteLine("\t{0}", entry.Value.ToString());
            }
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
