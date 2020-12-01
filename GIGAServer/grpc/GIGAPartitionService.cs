﻿using GIGAPartitionProto;
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
            Console.WriteLine($"Received vote for partition {request.PartitionId} from server {request.ServerId}");

            Console.WriteLine($"{gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.VoteReplyDecision(request.Term)}");

            return Task.FromResult(new VoteReply { VoteForCandidate = gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.VoteReplyDecision(request.Term), ServerId = gigaPartitionService.GigaServerService.Server.Name});
        }


        public override Task<SendLeaderReply> SendLeader(SendLeaderRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received new leader for partition {request.PartitionId} from server {request.ServerId}");

            gigaPartitionService.Partitions[request.PartitionId].Partition.RaftObject.AcceptNewLeader(request.ServerId ,gigaPartitionService.Partitions[request.PartitionId].Partition);

            return Task.FromResult(new SendLeaderReply { Ok = true });
        }
    }
}

