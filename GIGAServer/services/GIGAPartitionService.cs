﻿using GIGAServer.domain;
using GIGAServer.dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GIGAServer.services
{
    class GIGAPartitionService
    {
        private Dictionary<string, GIGAPartition> partitions = new Dictionary<string, GIGAPartition>();
        public int ReplicationFactor { get; set; }

        private GIGAServerService gigaServerService;

        public GIGAPartitionService(GIGAServerService gigaServerService)
        {
            this.gigaServerService = gigaServerService;
        }

        public bool RegisterPartition(string id, int replicationFactor, GIGAServerObject[] servers)
        {
            if (partitions.ContainsKey(id)) return false;

            partitions.Add(id, new GIGAPartition(id, replicationFactor, servers));

            return true;
        }

        internal GIGAObject Read(string partitionId, string objectId)
        {
            if (!partitions.ContainsKey(partitionId)) return null;
            return partitions[partitionId].Read(objectId);
        }

        internal void ShowStatus()
        {
            Console.WriteLine("Current Partitions:");
            foreach (KeyValuePair<string, GIGAPartition> entry in partitions)
            {
                entry.Value.ShowStatus();
            }
        }

        internal List<GIGAPartitionObjectID> ListObjects(string serverId)
        {
            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in partitions.Values.Where(p => p.HasServer(serverId)))
            {
                partition.Write("obj-1", "value-1");
                Console.WriteLine("ADDED");
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            return result;
        }

        internal List<GIGAPartitionObjectID> ListGlobal()
        {
            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in partitions.Values)
            {
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            return result;
        }

    }
}
