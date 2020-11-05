using GIGAServer.domain;
using GIGAServerProto;
using Grpc.Core;
using System;
using System.Collections.Generic;
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
            GIGAObject obj = gigaPartitionService.Read(request.PartitionId, request.ObjectId);
            ReadReply reply = new ReadReply { Ok = false };

            if (obj != null)
                reply = new ReadReply { Value = obj.Value, ObjectId = obj.Name, PartitionId = obj.Partition.Name, Ok = true };

            return Task.FromResult(reply);
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            KeyValuePair<bool, string> result = gigaPartitionService.Write(request.PartitionId, request.ObjectId, request.Value);
            WriteReply reply = new WriteReply { Ok = result.Key, MasterServer = result.Value };

            return Task.FromResult(reply);
        }

        public override Task<ListServerReply> ListServer(ListServerRequest request, ServerCallContext context)
        {
            ListServerReply reply = new ListServerReply();

            reply.Objects.AddRange(gigaPartitionService.ListObjects(request.ServerId).Select(objectId => new PartitionObjectID
            {
                PartitionId = objectId.PartitionName,
                ObjectId = objectId.ObjectName,
                IsMaster = objectId.MasterServerName == gigaServerService.Server.Name
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
