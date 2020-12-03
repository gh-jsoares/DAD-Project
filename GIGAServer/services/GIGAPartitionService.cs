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
        internal GIGAServerService GigaServerService { get; set; }

        public GIGAPartitionService(GIGAServerService gigaServerService)
        {
            this.GigaServerService = gigaServerService;

         
        }


        public void CheckFrozenServer()
        {
            Random random = new Random();

            // ADD RANDOM DELAY BETWEEN MIN AND MAX
            Thread.Sleep(random.Next(GigaServerService.MinDelay, GigaServerService.MaxDelay));

            Thread.CurrentThread.Name = string.Format("Thread {0}", Thread.CurrentThread.ManagedThreadId);

            if (GigaServerService.FreezeQueue.Count > 0 || GigaServerService.Frozen)
                GigaServerService.FreezeQueue.Enqueue(Thread.CurrentThread.Name);

            while(GigaServerService.FreezeQueue.Count > 0)
            {
                GigaServerService.FreezeQueuePopEvent.WaitOne();
                if (Thread.CurrentThread.Name == GigaServerService.FreezePoppedQueue) break;
                Thread.Sleep(random.Next(100, 500));
            }

            if (GigaServerService.FreezeQueue.Count > 0)
                GigaServerService.FreezeQueuePopEvent.Reset();
        }

        public void CheckFrozenServerHeartbeat()
        {
            Random random = new Random();

            // ADD RANDOM DELAY BETWEEN MIN AND MAX
            Thread.Sleep(random.Next(GigaServerService.MinDelay, GigaServerService.MaxDelay));

            Thread.CurrentThread.Name = string.Format("Thread {0}", Thread.CurrentThread.ManagedThreadId);

            if (GigaServerService.FreezeQueueHeartbeat.Count > 0 || GigaServerService.Frozen)
                GigaServerService.FreezeQueueHeartbeat.Enqueue(Thread.CurrentThread.Name);

            while (GigaServerService.FreezeQueueHeartbeat.Count > 0)
            {
                GigaServerService.FreezeQueueHeartbeatPopEvent.WaitOne();
                if (Thread.CurrentThread.Name == GigaServerService.FreezePoppedQueueHeartbeat) break;
                Thread.Sleep(random.Next(100, 500));
            }

            if (GigaServerService.FreezeQueueHeartbeat.Count > 0)
                GigaServerService.FreezeQueuePopEvent.Reset();
        }

        internal void LockWrite()
        {
            GigaServerService.WriteQueue.Enqueue(Thread.CurrentThread.Name);
        }

        internal void PerformWrite(string partitionId, string objectId, string value)
        {
            Partitions[partitionId].PerformWrite(objectId, value);
            GigaServerService.PopWriteQueue();
        }

        public void CheckWriteServer()
        {
            Random random = new Random();

            if (GigaServerService.WriteQueue.Count > 0)
                GigaServerService.WriteQueue.Enqueue(Thread.CurrentThread.Name);

            while(GigaServerService.WriteQueue.Count > 0)
            {
                GigaServerService.WriteQueuePopEvent.WaitOne();
                if (Thread.CurrentThread.Name == GigaServerService.WritePoppedQueue) break;
                Thread.Sleep(random.Next(100, 500));
            }

            if (GigaServerService.WriteQueue.Count > 0)
                GigaServerService.WriteQueuePopEvent.Reset();
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

            GigaServerService.PopWriteQueue();
            GigaServerService.PopFreezeQueue();

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

            GigaServerService.PopWriteQueue();
            GigaServerService.PopFreezeQueue();

            return result;
        }

        internal KeyValuePair<bool, string> Write(string partitionId, string objectId, string value)
        {
            CheckFrozenServer();
            CheckWriteServer();
            
            Console.WriteLine("WRITE");

            GIGAPartition partition = Partitions[partitionId];
            if (!partition.IsMaster(GigaServerService.Server))
            {
                return new KeyValuePair<bool, string>(false, partition.Master.Name);
            }
            partition.Write(objectId, value);

            GigaServerService.PopWriteQueue();
            GigaServerService.PopFreezeQueue();

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

            GigaServerService.PopWriteQueue();
            GigaServerService.PopFreezeQueue();

            return result;
        }

    }
}
