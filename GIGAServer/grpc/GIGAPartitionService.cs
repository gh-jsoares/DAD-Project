using GIGAPartitionProto;
using GIGAServerProto;
using Grpc.Core;
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
            return base.LockObject(request, context);
        }

        public override Task<WriteObjectReply> WriteObject(WriteObjectRequest request, ServerCallContext context)
        {
            return base.WriteObject(request, context);
        }
    }
}
