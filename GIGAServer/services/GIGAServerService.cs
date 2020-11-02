using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GIGAServer.services
{
    class GIGAServerService : GIGAServerProtoService.GIGAServerProtoServiceBase
    {
        public override Task<TestReply> Test(TestRequest request, ServerCallContext context)
        {
            return base.Test(request, context);
        }
    }
}
