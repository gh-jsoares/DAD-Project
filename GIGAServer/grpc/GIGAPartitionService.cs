using GIGAPartitionProto;
using GIGAServerProto;
using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GIGAServer.grpc
{
    class GIGAPartitionService : GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceBase
    {
        private services.GIGAPartitionService gigaPartitionService;

        public GIGAPartitionService(services.GIGAPartitionService gigaPartitionService)
        {
            this.gigaPartitionService = gigaPartitionService;
        }

        public override Task<LockObjectReply> LockObject(LockObjectRequest request, ServerCallContext context)
        {
            gigaPartitionService.LockWrite();
            return Task.FromResult(new LockObjectReply { Ok = true } );
        }

        public override Task<WriteObjectReply> WriteObject(WriteObjectRequest request, ServerCallContext context)
        {
            gigaPartitionService.PerformWrite(request.PartitionId, request.ObjectId, request.Value);
            return Task.FromResult(new WriteObjectReply { Ok = true }) ;
        }

        public override Task<VoteReply> Vote(VoteRequest request, ServerCallContext context)
        {

            gigaPartitionService.CheckFrozenServer();

            Console.WriteLine($"Received vote for partition {request.PartitionId} from server {request.ServerId}");

            bool voteForCandidate = gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.VoteReplyDecision(request);

            Console.WriteLine($"{voteForCandidate}");

            gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.CheckTerm(request.Term);

            if(voteForCandidate)
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
            Console.WriteLine($"Received new leader for partition {request.PartitionId} from server {request.ServerId}");

            gigaPartitionService.CheckFrozenServerHeartbeat();

            gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.AcceptNewLeader(request.ServerId ,gigaPartitionService.Partitions[request.PartitionId].Partition);

            gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.CheckTerm(request.Term);

            return Task.FromResult(new SendLeaderReply { Term = gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.Term, Ok = true });
        }

        public override Task<ServerCrashReply> ServerCrash(ServerCrashRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received new server crashed: {request.ServerId}");

            gigaPartitionService.CheckFrozenServer();

            gigaPartitionService.Partitions[request.PartitionId].Partition.RemoveServer(request.ServerId);
            gigaPartitionService.Partitions[request.PartitionId].PartitionMap.Remove(request.ServerId);

            return Task.FromResult(new ServerCrashReply { Ok = true });
        }
    }
}

