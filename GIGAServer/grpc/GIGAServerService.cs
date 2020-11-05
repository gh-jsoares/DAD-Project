using GIGAServerProto;
using Grpc.Core;
using System.Threading.Tasks;

namespace GIGAServer.grpc
{
    class GIGAServerService : GIGAServerProto.GIGAServerService.GIGAServerServiceBase
    {
        private services.GIGAServerService gigaServerService;

        public GIGAServerService(services.GIGAServerService gigaServerService)
        {
            this.gigaServerService = gigaServerService;
        }
        public override Task<GIGAServerProto.TestReply> Test(GIGAServerProto.TestRequest request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            //TODO: mudar o tipo de objectid de string para GIGAObject no GIGAServer.proto
           // domain.GIGAObject obj = gigaServerService.Read(request.PartitionId, request.ObjectId);
           // GigaPartitionObject partition = new GigaPartitionObject { Name = obj.Name, RepFactor = obj.} ;
            return Task.FromResult(new ReadReply { Value = gigaServerService.Read(request.PartitionId, request.ObjectId)});
                      
        }

        public override Task<ListServerReply> ListServer(ListServerRequest request, ServerCallContext context)
        {
            //TODO gigaServer.Service.ListServer()
            return Task.FromResult(new ListServerReply {});
        }
       
    }
}
