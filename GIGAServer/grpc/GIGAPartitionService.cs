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
            Console.WriteLine($"Received vote for partition {request.PartitionId} from server {request.ServerId}");

            Random r = new Random();

            Thread.Sleep(r.Next(0, 10000));

            return Task.FromResult(new VoteReply { VoteForCandidate = true });
        }
    }
}

