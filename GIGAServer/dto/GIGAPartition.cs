using GIGAPartitionProto;
using GIGAServer.domain;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;

namespace GIGAServer.dto
{
    class GIGAPartition
    {
        public GIGAServerObject Master { get { return Partition.MasterServer; } }

        internal GIGAPartitionObject Partition { get; set; }
        public Dictionary<string, GIGAPartitionService.GIGAPartitionServiceClient> PartitionMap { get; set; } = new Dictionary<string, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient>();

        public GIGAPartition(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Partition = new GIGAPartitionObject(name, replicationFactor, servers);
            foreach (GIGAServerObject server in servers)
            {
                PartitionMap.Add(server.Name, new GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient(GrpcChannel.ForAddress(server.Url)));
            }
        }

        internal void ShowStatus()
        {
            Partition.ShowStatus();
        }

        public GIGAObject Read(string name)
        {
            return Partition.Read(name);
        }

        internal List<GIGAPartitionObjectID> GetPartitionObjectIDList()
        {
            return Partition.GetPartitionObjectIDList();
        }

        internal bool HasServer(string serverId)
        {
            return Partition.HasServer(serverId);
        }

        internal void Write(string name, string value)
        {
            foreach (var partitionClient in PartitionMap.Values)
            {
                partitionClient.LockObject(new GIGAPartitionProto.LockObjectRequest { ObjectId = name, PartitionId = Partition.Name });
            }

            // Received all Acks
            PerformWrite(name, value);

            foreach (var partitionClient in PartitionMap.Values)
            {
                partitionClient.WriteObject(new GIGAPartitionProto.WriteObjectRequest { Value = value, ObjectId = name, PartitionId = Partition.Name });
            }
        }

        internal void PerformWrite(string objectId, string value)
        {
            Partition.Write(objectId, value);
        }

        internal bool IsMaster(GIGAServerObject server)
        {
            return server.Name == Master.Name;
        }



        //RAFT GRPC

        internal void NewRaftObject()
        {
            Partition.CreateRaftObject();
        }
        internal bool SendVoteRequest(string server_id, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient partitionClient)
        {

            GIGAPartitionProto.VoteReply vote = partitionClient.Vote(new GIGAPartitionProto.VoteRequest { PartitionId = Partition.Name, ServerId = server_id });

            Console.WriteLine($"Received {vote}");

            return true;
        }

    }
}
