using System;
using System.Linq;
using System.Threading.Tasks;
using GIGAPartitionProto;
using GIGAServer.domain;
using Grpc.Core;

namespace GIGAServer.grpc
{
    internal class GIGAPartitionService : GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceBase
    {
        private readonly services.GIGAPartitionService gigaPartitionService;

        public GIGAPartitionService(services.GIGAPartitionService gigaPartitionService)
        {
            this.gigaPartitionService = gigaPartitionService;
        }

        public override Task<VoteReply> Vote(VoteRequest request, ServerCallContext context)
        {
            gigaPartitionService.CheckFrozenServer();

            Console.WriteLine($"Received vote for partition {request.PartitionId} from server {request.ServerId}");

            var voteForCandidate = gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject
                .VoteReplyDecision(request, gigaPartitionService.Partitions[request.PartitionId].Partition);

            Console.WriteLine($"{voteForCandidate.ToString()}");

            gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.CheckTerm(request.Term);

            if (voteForCandidate)
                gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.votedFor = request.ServerId;

            return Task.FromResult(new VoteReply
            {
                Term = gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.Term,
                VoteForCandidate = voteForCandidate,
                PartitionId = request.PartitionId,
                ServerId = gigaPartitionService.GigaServerService.Server.Name
            });
        }


        public override Task<SendLeaderReply> SendLeader(SendLeaderRequest request, ServerCallContext context)
        {
            Console.WriteLine(
                $"Received new leader for partition {request.PartitionId} from server {request.ServerId}");

            gigaPartitionService.CheckFrozenServerHeartbeat();

            gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.AcceptNewLeader(request.ServerId,
                request.Term,
                gigaPartitionService.Partitions[request.PartitionId].Partition);

            gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.CheckTerm(request.Term);

            return Task.FromResult(new SendLeaderReply
                {Term = gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.Term, Ok = true});
        }

        public override Task<ServerCrashReply> ServerCrash(ServerCrashRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received new server crashed: {request.ServerId}");

            gigaPartitionService.CheckFrozenServer();

            gigaPartitionService.Partitions[request.PartitionId].Partition.RemoveServer(request.ServerId);
            gigaPartitionService.Partitions[request.PartitionId].PartitionMap.Remove(request.ServerId);

            return Task.FromResult(new ServerCrashReply {Ok = true});
        }

        public override Task<AppendEntriesReply> AppendEntries(AppendEntriesRequest request, ServerCallContext context)
        {
            Console.WriteLine(
                $"Received new append entries from leader {request.ServerId} in partition {request.PartitionId}");
            gigaPartitionService.CheckFrozenServerHeartbeat();
            var appendEntriesReply = new AppendEntriesReply();
            Console.WriteLine(request);

            try
            {
                var partitionObject = gigaPartitionService.Partitions[request.PartitionId];

                var lastCommittedEntries = request.LastCommittedLogs.Select(lastCommittedLog =>
                    new GIGALogEntry(lastCommittedLog.Term, lastCommittedLog.Log,
                        new GIGAObject(partitionObject.Partition, lastCommittedLog.ObjectId, lastCommittedLog.Value,
                            lastCommittedLog.Log)));

                GIGALogEntry entry = null;
                if (request.Entry != null)
                    entry = new GIGALogEntry(request.Entry.Term, request.Entry.Log,
                        new GIGAObject(partitionObject.Partition, request.Entry.ObjectId, request.Entry.Value,
                            request.Entry.Log));


                partitionObject.Partition.RaftObject.AcceptNewLeader(request.ServerId, request.Term,
                    partitionObject.Partition);
                partitionObject.Partition.RaftObject.CheckTerm(request.Term);

                var lastCommittedLog = partitionObject.Partition.AppendEntries(entry, lastCommittedEntries.ToList());

                appendEntriesReply.Ok = lastCommittedLog == null;
                appendEntriesReply.Term = partitionObject.Partition.RaftObject.Term;

                if (lastCommittedLog != null)
                    appendEntriesReply.LastCommittedLog = new LogEntryProto
                    {
                        Log = lastCommittedLog.Index,
                        ObjectId = lastCommittedLog.Data.Name,
                        Term = lastCommittedLog.Term,
                        Value = lastCommittedLog.Data.Value
                    };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                throw;
            }

            return Task.FromResult(appendEntriesReply);
        }
    }
}