using GIGAServer.domain;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace GIGAServer.dto
{
    class GIGAPartition
    {
        private GIGAPartitionObject partition;
        private Dictionary<string, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient> partitionMap =
            new Dictionary<string, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient>();

        public GIGAServerObject Master { get { return partition.MasterServer; } }

        public GIGAPartition(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            partition = new GIGAPartitionObject(name, replicationFactor, servers);
            foreach (GIGAServerObject server in servers)
            {
                partitionMap.Add(server.Name, new GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient(GrpcChannel.ForAddress(server.Url)));
            }
        }

        internal void ShowStatus()
        {
            partition.ShowStatus();
        }

        public GIGAObject Read(string name)
        {
            return partition.Read(name);
        }

        internal List<GIGAPartitionObjectID> GetPartitionObjectIDList()
        {
            return partition.GetPartitionObjectIDList();
        }

        internal bool HasServer(string serverId)
        {
            return partition.HasServer(serverId);
        }

        internal void Write(string name, string value)
        {
            foreach (var partitionClient in partitionMap.Values)
            {
                partitionClient.LockObject(new GIGAPartitionProto.LockObjectRequest { ObjectId = name, PartitionId = partition.Name });
            }

            // Received all Acks
            PerformWrite(name, value);

            foreach (var partitionClient in partitionMap.Values)
            {
                partitionClient.WriteObject(new GIGAPartitionProto.WriteObjectRequest { Value = value, ObjectId = name, PartitionId = partition.Name });
            }
        }

        internal void PerformWrite(string objectId, string value)
        {
            partition.Write(objectId, value);
        }

        internal bool IsMaster(GIGAServerObject server)
        {
            return server.Name == Master.Name;
        }
    }
}
