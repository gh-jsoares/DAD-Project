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
        public Dictionary<string, GIGAServerObject> Servers { get; }
        public int ReplicationFactor { get; }
        public GIGAServerObject MasterServer { get; }
        internal GIGARaftObject RaftObject { get; set; }

        private Dictionary<string, GIGAObject> objects;

        public GIGAPartitionObject(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            if (servers == null) throw new ArgumentNullException(nameof(servers));
            Servers = servers.ToDictionary(server => server.Name, server => server);
            objects = new Dictionary<string, GIGAObject>();
            MasterServer = servers.First();    
        }

        internal void ShowStatus()
        {
            Console.WriteLine("\tPartition \"{0}\":\n\t\tCurrent Master: {1}", Name, MasterServer.ToString());
            foreach (KeyValuePair<string, GIGAServerObject> entry in Servers)
            {
                Console.WriteLine("\t\t{0}", entry.Value.ToString());
            }
        }

        internal GIGAObject Read(string name)
        {
            // TODO LOCKED
            return GetObject(name);
        }

        internal bool HasServer(string serverId)
        {
            return Servers.ContainsKey(serverId);
        }

        public void Write(string name, string value)
        {
            if (objects.ContainsKey(name))
                objects[name].Value = value;
            else
                objects.Add(name, new GIGAObject(this, name, value));
        }

        internal List<GIGAPartitionObjectID> GetPartitionObjectIDList()
        {
            return objects.Values.Select(obj => obj.ToPartitionObjectID()).ToList();
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


        //Raft
        public void CreateRaftObject()
        {
            this.RaftObject = new GIGARaftObject(Servers);
        }
    }
}
