using Grpc.Core;
using System.Threading.Tasks;

namespace GIGAServer.grpc
{
    class GIGAServerService : GIGAServerProto.GIGAServerService.GIGAServerServiceBase
    {
        public override Task<GIGAServerProto.TestReply> Test(GIGAServerProto.TestRequest request, ServerCallContext context)
        {
            return base.Test(request, context);
        }
    }
}
