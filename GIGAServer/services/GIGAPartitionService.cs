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
            Random random = new Random();
            Thread.CurrentThread.Name = string.Format("Thread {0}", gigaServerService.FreezeQueue.Count);
            Console.WriteLine("Hello");
            if (gigaServerService.FreezeQueue.Count > 0 || gigaServerService.Frozen)
                gigaServerService.FreezeQueue.Enqueue(Thread.CurrentThread.Name);

            while(gigaServerService.FreezeQueue.Count > 0)
            {
                gigaServerService.QueuePopEvent.WaitOne();
                if (Thread.CurrentThread.Name == gigaServerService.PoppedQueue) break;
                Thread.Sleep(random.Next(100, 500));
            }

            if (gigaServerService.FreezeQueue.Count > 0)
                gigaServerService.QueuePopEvent.Reset();
        }

        public bool RegisterPartition(string id, int replicationFactor, GIGAServerObject[] servers)
        {
            Console.WriteLine("REGISTER PARTITION");
            if (partitions.ContainsKey(id)) return false;

            partitions.Add(id, new GIGAPartition(id, replicationFactor, servers));

            return true;
        }

        internal GIGAObject Read(string partitionId, string objectId)
        {
            CheckFrozenServer();
            Console.WriteLine("READ");
            if (!partitions.ContainsKey(partitionId)) return null;
            GIGAObject obj = partitions[partitionId].Read(objectId);
            gigaServerService.PopQueue();
            return obj;
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
            CheckFrozenServer();

            Console.WriteLine("LIST OBJECTS");

            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in partitions.Values.Where(p => p.HasServer(serverId)))
            {
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            gigaServerService.PopQueue();
            return result;
        }

        internal KeyValuePair<bool, string> Write(string partitionId, string objectId, string value)
        {
            CheckFrozenServer();
            
            Console.WriteLine("WRITE");

            GIGAPartition partition = partitions[partitionId];
            if (!partition.IsMaster(gigaServerService.Server))
            {
                return new KeyValuePair<bool, string>(false, partition.Master.Name);
            }
            partition.Write(objectId, value);
            gigaServerService.PopQueue();
            return new KeyValuePair<bool, string>(true, "");
        }

        internal List<GIGAPartitionObjectID> ListGlobal()
        {
            CheckFrozenServer();
            Console.WriteLine("LIST GLOBAL");
            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in partitions.Values)
            {
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            gigaServerService.PopQueue();
            return result;
        }

    }
}
