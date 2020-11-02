using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAServer
{
    class GIGAServerLogic
    {
        private Dictionary<string, GIGAPartitionObject> partitions = new Dictionary<string, GIGAPartitionObject>();
        private GIGAServerObject server;
        public int ReplicationFactor { get; set; }
        private bool frozen = false;

        public GIGAServerLogic(string id)
        {
            server = new GIGAServerObject(id);
        }

        public GIGAObject Read(string partitionId, string objectId)
        {
            // TODO
            return null;
        }

        public void Write(string partitionId, GIGAObject value)
        {
            // TODO
        }

        public void RegisterPartition(string id, int replicationFactor, GIGAServerObject[] servers)
        {
            partitions.Add(id, new GIGAPartitionObject(id, replicationFactor, servers));
        }

        public void Unfreeze()
        {
            frozen = false;
        }

        public void Freeze()
        {
            frozen = true;
        }

        public void ShowStatus()
        {
            // TODO PRINT STATUS INFORMATION TO CONSOLE
        }
    }
}
