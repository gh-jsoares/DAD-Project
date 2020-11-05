using GIGAServer.domain;
using GIGAServer.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public void CheckFrozenServer()
        {
            while(gigaServerService.Frozen)
            {
                Thread.Sleep(2000);
            }
        }

        public bool RegisterPartition(string id, int replicationFactor, GIGAServerObject[] servers)
        {
            CheckFrozenServer();
            if (partitions.ContainsKey(id)) return false;

            partitions.Add(id, new GIGAPartition(id, replicationFactor, servers));

            return true;
        }

        internal GIGAObject Read(string partitionId, string objectId)
        {
            CheckFrozenServer();
            if (!partitions.ContainsKey(partitionId)) return null;
            return partitions[partitionId].Read(objectId);
        }

        internal void ShowStatus()
        {
            CheckFrozenServer();
            Console.WriteLine("Current Partitions:");
            foreach (KeyValuePair<string, GIGAPartition> entry in partitions)
            {
                entry.Value.ShowStatus();
            }
        }

        internal List<GIGAPartitionObjectID> ListObjects(string serverId)
        {
            CheckFrozenServer();
            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in partitions.Values.Where(p => p.HasServer(serverId)))
            {
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            return result;
        }

        internal KeyValuePair<bool, string> Write(string partitionId, string objectId, string value)
        {
            CheckFrozenServer();
            Console.WriteLine("Added");
            GIGAPartition partition = partitions[partitionId];
            if (!partition.IsMaster(gigaServerService.Server))
            {
                return new KeyValuePair<bool, string>(false, partition.Master.Name);
            }
            partition.Write(objectId, value);
            return new KeyValuePair<bool, string>(true, "");
        }

        internal List<GIGAPartitionObjectID> ListGlobal()
        {
            CheckFrozenServer();
            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in partitions.Values)
            {
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            return result;
        }

    }
}
