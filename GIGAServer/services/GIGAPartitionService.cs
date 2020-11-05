using GIGAServer.domain;
using GIGAServer.dto;
using System;
using System.Collections.Generic;

namespace GIGAServer.services
{
    class GIGAPartitionService
    {
        private Dictionary<string, GIGAPartition> partitions = new Dictionary<string, GIGAPartition>();
        public int ReplicationFactor { get; set; }


        public bool RegisterPartition(string id, int replicationFactor, GIGAServerObject[] servers)
        {
            if (partitions.ContainsKey(id)) return false;

            partitions.Add(id, new GIGAPartition(id, replicationFactor, servers));

            return true;
        }

        internal void ShowStatus()
        {
            Console.WriteLine("Current Partitions:");
            foreach (KeyValuePair<string, GIGAPartition> entry in partitions)
            {
                entry.Value.ShowStatus();
            }
        }
    }
}
