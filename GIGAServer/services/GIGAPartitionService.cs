using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GIGAServer.domain;
using GIGAServer.dto;

namespace GIGAServer.services
{
    internal class GIGAPartitionService
    {
        private GIGARaftService gigaRaftService;

        public GIGAPartitionService(GIGAServerService gigaServerService)
        {
            GigaServerService = gigaServerService;
        }

        public int ReplicationFactor { get; set; }
        internal Dictionary<string, GIGAPartition> Partitions { get; set; } = new Dictionary<string, GIGAPartition>();
        internal GIGAServerService GigaServerService { get; set; }

        public void InitRaftService(GIGARaftService raftService)
        {
            gigaRaftService = raftService;
        }

        public void CheckFrozenServer()
        {
            var random = new Random();

            // ADD RANDOM DELAY BETWEEN MIN AND MAX
            Thread.Sleep(random.Next(GigaServerService.MinDelay, GigaServerService.MaxDelay));

            Thread.CurrentThread.Name = string.Format("Thread {0}", Thread.CurrentThread.ManagedThreadId);

            if (GigaServerService.FreezeQueue.Count > 0 || GigaServerService.Frozen)
                GigaServerService.FreezeQueue.Enqueue(Thread.CurrentThread.Name);

            while (GigaServerService.FreezeQueue.Count > 0)
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
            var random = new Random();

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

        public void CheckWriteServer()
        {
            var random = new Random();

            if (GigaServerService.WriteQueue.Count > 0 || GigaServerService.IsWriting)
                GigaServerService.WriteQueue.Enqueue(Thread.CurrentThread.Name);

            while (GigaServerService.WriteQueue.Count > 0)
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
            if (Partitions.ContainsKey(id)) return false;
            Console.WriteLine("REGISTER PARTITION");

            Partitions.Add(id, new GIGAPartition(id, replicationFactor, servers, GigaServerService.Server));

            return true;
        }

        internal GIGAObject Read(string partitionId, string objectId)
        {
            CheckFrozenServer();

            Console.WriteLine("READ");
            if (!Partitions.ContainsKey(partitionId)) return null;
            var obj = Partitions[partitionId].Read(objectId);

            GigaServerService.PopFreezeQueue();

            return obj;
        }

        internal void ShowStatus()
        {
            Console.WriteLine("Current Partitions:");
            foreach (var entry in Partitions) entry.Value.ShowStatus();
        }

        internal KeyValuePair<bool, string> Write(string partitionId, string objectId, string value)
        {
            CheckFrozenServer();

            Console.WriteLine("WRITE");

            var partition = Partitions[partitionId];
            if (!partition.IsMaster(GigaServerService.Server))
            {
                Console.WriteLine(partition.GetMaster());
                GigaServerService.PopFreezeQueue();
                return new KeyValuePair<bool, string>(false, partition.GetMaster());
            }

            // wait for raft service to startup
            while (gigaRaftService == null) Thread.Sleep(100);

            CheckWriteServer();
            GigaServerService.IsWriting = true;
            var entry = partition.Partition.CreateLog(objectId, value);
            gigaRaftService.BroadcastAppendEntries(partition, entry);

            // Commit entry and replace read value
            partition.Partition.CommitEntry(entry);

            GigaServerService.IsWriting = false;

            GigaServerService.PopWriteQueue();
            GigaServerService.PopFreezeQueue();

            return new KeyValuePair<bool, string>(true, "");
        }

        public List<GIGAPartition> ListPartitions()
        {
            CheckFrozenServer();

            Console.WriteLine("LIST SERVER");

            var result = Partitions.Values.ToList();

            GigaServerService.PopFreezeQueue();

            return result;
        }
    }
}