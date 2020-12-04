using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GIGAPartitionProto;
using GIGAServer.domain;
using Grpc.Net.Client;

namespace GIGAServer.dto
{
    internal class GIGAPartition
    {
        public GIGAPartition(string name, int replicationFactor, GIGAServerObject[] servers,
            GIGAServerObject thisServer)
        {
            Partition = new GIGAPartitionObject(name, replicationFactor, servers);
            foreach (var server in servers)
                if (server.Name != thisServer.Name)
                    PartitionMap.Add(server.Name,
                        new GIGAPartitionService.GIGAPartitionServiceClient(
                            GrpcChannel.ForAddress(server.Url)));
        }

        public GIGAServerObject Master() => Partition.MasterServer;

        internal GIGAPartitionObject Partition { get; set; }

        public Dictionary<string, GIGAPartitionService.GIGAPartitionServiceClient> PartitionMap { get; set; } =
            new Dictionary<string, GIGAPartitionService.GIGAPartitionServiceClient>();

        internal void ShowStatus()
        {
            Partition.ShowStatus();
        }

        public GIGALogEntry Read(string name)
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

        internal bool IsMaster(GIGAServerObject server)
        {
            return server.Name == GetMaster();
        }


        //RAFT GRPC

        internal void NewRaftObject()
        {
            Partition.CreateRaftObject();
        }

        internal void SendVoteRequest(string server_id, string receiver_id,
            GIGAPartitionService.GIGAPartitionServiceClient partitionClient)
        {
            try
            {
                var reply = partitionClient.Vote(
                    new VoteRequest
                    {
                        Term = Partition.RaftObject.Term,
                        ServerId = server_id,
                        LastLogIndex = Partition.GetLastLogIndex(),
                        LastLogTerm = Partition.GetLastLogTerm(),
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
            catch (Exception e)
            {
                Console.WriteLine($"{receiver_id} has crashed!");

                HandleServerCrash(receiver_id, e);
            }
        }

        internal void SendNewLeader(string server_id, string receiver_id,
            GIGAPartitionService.GIGAPartitionServiceClient partitionClient)
        {
            try
            {
                var reply = partitionClient.SendLeader(
                    new SendLeaderRequest
                    {
                        Term = Partition.RaftObject.Term,
                        PartitionId = Partition.Name,
                        ServerId = server_id
                    });

                Partition.RaftObject.CheckTerm(reply.Term);

                Console.WriteLine($"Received LeaderReply {reply}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{receiver_id} has crashed!");

                HandleServerCrash(receiver_id, e);
            }
        }

        public void SendAppendNewEntries(string server_id, string receiver_id,
            GIGAPartitionService.GIGAPartitionServiceClient partitionClient,
            GIGALogEntry entry)
        {
            try
            {
                var lastCommittedLogs = new List<GIGALogEntry>();

                GIGALogEntry committedLog;
                if ((committedLog = Partition.GetLastCommittedLog()) != null)
                {
                    lastCommittedLogs.Add(committedLog);
                }

                var ok = false;

                while (!ok)
                {
                    var appendEntriesRequest = new AppendEntriesRequest
                    {
                        PartitionId = Partition.Name,
                        ServerId = server_id,
                        LastCommittedLogs =
                        {
                            lastCommittedLogs.Select(log => new LogEntryProto
                            {
                                Log = log.Index,
                                ObjectId = log.Data.Name,
                                Term = log.Term,
                                Value = log.Data.Value
                            })
                        },
                        Term = Partition.RaftObject.Term
                    };

                    // if not just heartbeat, it is a new entry
                    if (entry != null)
                    {
                        appendEntriesRequest.Entry = new LogEntryProto
                        {
                            Log = entry.Index,
                            ObjectId = entry.Data.Name,
                            Term = entry.Term,
                            Value = entry.Data.Value
                        };
                    }


                    var reply = partitionClient.AppendEntries(appendEntriesRequest);
                    ok = reply.Ok;

                    // if not ok, it means i need to send one more log from before
                    if (!ok)
                        lastCommittedLogs.Insert(0,
                            Partition.GetLastCommittedLogBefore(reply.LastCommittedLog.Log,
                                reply.LastCommittedLog.Term));

                    if (reply.LastCommittedLog != null)
                        Partition.RaftObject.CheckTerm(reply.LastCommittedLog.Term);

                    Console.WriteLine($"Received LeaderReply {reply}");
                }

                // Sent successfully
            }
            catch (Exception e)
            {
                Console.WriteLine($"{receiver_id} has crashed!");

                HandleServerCrash(receiver_id, e);
            }
        }

        internal void HandleServerCrash(string server_id, Exception e)
        {
            Console.Error.WriteLine(e.StackTrace);
            Console.Error.WriteLine(e.Message);
            Partition.RemoveServer(server_id);
            PartitionMap.Remove(server_id);

            //Alert remaining servers of partition
            foreach (var partitionClient_2 in PartitionMap)
            {
                var serverCrashThread = new Thread(() => ServerCrash(server_id, partitionClient_2.Value));
                serverCrashThread.Start();
            }
        }

        internal void ServerCrash(string server_id,
            GIGAPartitionService.GIGAPartitionServiceClient partitionClient)
        {
            var reply = partitionClient.ServerCrash(
                new ServerCrashRequest
                {
                    ServerId = server_id,
                    PartitionId = Partition.Name
                });
        }

        public string GetMaster()
        {
            return Master()?.Name;
        }
    }
}