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

        internal void SendVoteRequest(string server_id, string receiver_id, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient partitionClient)
        {
            try
            {
                GIGAPartitionProto.VoteReply reply = partitionClient.Vote(
                new GIGAPartitionProto.VoteRequest
                {
                    Term = Partition.RaftObject.Term,
                    ServerId = server_id,
                    LastLogIndex = 1,
                    LastLogTerm = 1,
                    PartitionId = Partition.Name
                });

                Console.WriteLine($"Received VoteReply {reply}");

                if (Partition.RaftObject.State == 2)
                {
                    Partition.RaftObject.HandleVoteReply(reply);

                    Partition.RaftObject.CheckTerm(reply.Term);

                    lock (Partition.RaftObject)
                    {
                        Monitor.PulseAll(Partition.RaftObject);
                    }
                }
            }
            catch
            {
                Console.WriteLine($"{receiver_id} has crashed!");

                HandleServerCrash(receiver_id);
            }
            
            
                
        }

        internal void SendNewLeader(string server_id, string receiver_id, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient partitionClient)
        {

            try
            {
                GIGAPartitionProto.SendLeaderReply reply = partitionClient.SendLeader(
                new GIGAPartitionProto.SendLeaderRequest
                {
                    Term = Partition.RaftObject.Term,
                    PartitionId = Partition.Name,
                    ServerId = server_id
                });

                Partition.RaftObject.CheckTerm(reply.Term);

                Console.WriteLine($"Received LeaderReply {reply}");
            }
            catch(Exception e)
            {
                Console.WriteLine($"{receiver_id} has crashed!");

                HandleServerCrash(receiver_id);
            }
            
        }

        internal void HandleServerCrash(string server_id)
        {
            Partition.RemoveServer(server_id);
            PartitionMap.Remove(server_id);

            //Alert remaining servers of partition
            foreach (var partitionClient_2 in PartitionMap)
            {
                Thread serverCrashThread = new Thread(() => ServerCrash(server_id, partitionClient_2.Value));
                serverCrashThread.Start();
            }
        }

        internal void ServerCrash(string server_id, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient partitionClient)
        {
            GIGAPartitionProto.ServerCrashReply reply = partitionClient.ServerCrash(
                new GIGAPartitionProto.ServerCrashRequest
                {
                    ServerId = server_id,
                    PartitionId = Partition.Name
                });
        }
    }
}
