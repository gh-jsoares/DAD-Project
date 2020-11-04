using GIGAServer.domain;
using GIGAServer.logic;
using System.Collections.Generic;

namespace GIGAServer.services
{
    class GIGAPartitionService
    {
        private Dictionary<string, GIGAPartition> partitions = new Dictionary<string, GIGAPartition>();
        public int ReplicationFactor { get; set; }


        public void RegisterPartition(string id, int replicationFactor, GIGAServerObject[] servers)
        {
            partitions.Add(id, new GIGAPartition(id, replicationFactor, servers));
        }

    }
}
