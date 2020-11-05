using GIGAServerProto;
using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GIGAServer.grpc
{
    class GIGAServerService : GIGAServerProto.GIGAServerService.GIGAServerServiceBase
    {
        private services.GIGAServerService gigaServerService;
        private services.GIGAPartitionService gigaPartitionService;

        public GIGAServerService(services.GIGAServerService gigaServerService, services.GIGAPartitionService gigaPartitionService)
        {
            this.gigaServerService = gigaServerService;
            this.gigaPartitionService = gigaPartitionService;
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
            ListServerReply reply = new ListServerReply();

            reply.Objects.AddRange(gigaPartitionService.ListObjects(request.ServerId).Select(objectId => new PartitionObjectID
            {
                PartitionId = objectId.PartitionName,
                ObjectId = objectId.ObjectName,
                IsMaster = true
            }));

            return Task.FromResult(reply);
        }

        public override Task<ListGlobalReply> ListGlobal(ListGlobalRequest request, ServerCallContext context)
        {
            ListGlobalReply reply = new ListGlobalReply();

            reply.Objects.AddRange(gigaPartitionService.ListGlobal().Select(objectId => new PartitionObjectID { PartitionId = objectId.PartitionName, ObjectId = objectId.ObjectName }));

            return Task.FromResult(reply);
        }
    }
}
