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
        public int ReplicationFactor { get; set; }
        internal Dictionary<string, GIGAPartition> Partitions { get; set; } = new Dictionary<string, GIGAPartition>();

        private GIGAServerService gigaServerService;

        public GIGAPartitionService(GIGAServerService gigaServerService)
        {
            this.gigaServerService = gigaServerService;

         
        }


        public void CheckFrozenServer()
        {
            Random random = new Random();

            // ADD RANDOM DELAY BETWEEN MIN AND MAX
            Thread.Sleep(random.Next(gigaServerService.MinDelay, gigaServerService.MaxDelay));

            Thread.CurrentThread.Name = string.Format("Thread {0}", Thread.CurrentThread.ManagedThreadId);

            if (gigaServerService.FreezeQueue.Count > 0 || gigaServerService.Frozen)
                gigaServerService.FreezeQueue.Enqueue(Thread.CurrentThread.Name);

            while(gigaServerService.FreezeQueue.Count > 0)
            {
                gigaServerService.FreezeQueuePopEvent.WaitOne();
                if (Thread.CurrentThread.Name == gigaServerService.FreezePoppedQueue) break;
                Thread.Sleep(random.Next(100, 500));
            }

            if (gigaServerService.FreezeQueue.Count > 0)
                gigaServerService.FreezeQueuePopEvent.Reset();
        }

        internal void LockWrite()
        {
            gigaServerService.WriteQueue.Enqueue(Thread.CurrentThread.Name);
        }

        internal void PerformWrite(string partitionId, string objectId, string value)
        {
            Partitions[partitionId].PerformWrite(objectId, value);
            gigaServerService.PopWriteQueue();
        }

        public void CheckWriteServer()
        {
            Random random = new Random();

            if (gigaServerService.WriteQueue.Count > 0)
                gigaServerService.WriteQueue.Enqueue(Thread.CurrentThread.Name);

            while(gigaServerService.WriteQueue.Count > 0)
            {
                gigaServerService.WriteQueuePopEvent.WaitOne();
                if (Thread.CurrentThread.Name == gigaServerService.WritePoppedQueue) break;
                Thread.Sleep(random.Next(100, 500));
            }

            if (gigaServerService.WriteQueue.Count > 0)
                gigaServerService.WriteQueuePopEvent.Reset();
        }

        public bool RegisterPartition(string id, int replicationFactor, GIGAServerObject[] servers)
        {
            Console.WriteLine("REGISTER PARTITION");
            if (Partitions.ContainsKey(id)) return false;

            Partitions.Add(id, new GIGAPartition(id, replicationFactor, servers));

            return true;
        }

        internal GIGAObject Read(string partitionId, string objectId)
        {
            CheckFrozenServer();
            CheckWriteServer();

            Console.WriteLine("READ");
            if (!Partitions.ContainsKey(partitionId)) return null;
            GIGAObject obj = Partitions[partitionId].Read(objectId);

            gigaServerService.PopWriteQueue();
            gigaServerService.PopFreezeQueue();

            return obj;
        }

        internal void ShowStatus()
        {
            Console.WriteLine("Current Partitions:");
            foreach (KeyValuePair<string, GIGAPartition> entry in Partitions)
            {
                entry.Value.ShowStatus();
            }
        }

        internal List<GIGAPartitionObjectID> ListObjects(string serverId)
        {
            CheckFrozenServer();
            CheckWriteServer();

            Console.WriteLine("LIST OBJECTS");

            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in Partitions.Values.Where(p => p.HasServer(serverId)))
            {
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            gigaServerService.PopWriteQueue();
            gigaServerService.PopFreezeQueue();

            return result;
        }

        internal KeyValuePair<bool, string> Write(string partitionId, string objectId, string value)
        {
            CheckFrozenServer();
            CheckWriteServer();
            
            Console.WriteLine("WRITE");

            GIGAPartition partition = Partitions[partitionId];
            if (!partition.IsMaster(gigaServerService.Server))
            {
                return new KeyValuePair<bool, string>(false, partition.Master.Name);
            }
            partition.Write(objectId, value);

            gigaServerService.PopWriteQueue();
            gigaServerService.PopFreezeQueue();

            return new KeyValuePair<bool, string>(true, "");
        }

        internal List<GIGAPartitionObjectID> ListGlobal()
        {
            CheckFrozenServer();
            CheckWriteServer();

            Console.WriteLine("LIST GLOBAL");
            List<GIGAPartitionObjectID> result = new List<GIGAPartitionObjectID>();

            foreach (GIGAPartition partition in Partitions.Values)
            {
                result.AddRange(partition.GetPartitionObjectIDList());
            }

            gigaServerService.PopWriteQueue();
            gigaServerService.PopFreezeQueue();

            return result;
        }

    }
}
