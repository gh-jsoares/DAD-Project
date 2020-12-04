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

        public GIGAServerService(services.GIGAServerService gigaServerService,
            services.GIGAPartitionService gigaPartitionService)
        {
            this.gigaServerService = gigaServerService;
            this.gigaPartitionService = gigaPartitionService;
        }

        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            var reply = new ReadReply {Ok = false};
            try
            {
                GIGALogEntry obj = gigaPartitionService.Read(request.PartitionId, request.ObjectId);

                if (obj != null)
                    reply = new ReadReply
                        {Value = obj.Data.Value, ObjectId = obj.Data.Name, PartitionId = obj.Data.Partition.Name, Ok = true, Timestamp = obj.Index};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Task.FromResult(reply);
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            var reply = new WriteReply {Ok = false};
            try
            {
                var (ok, masterServerID) =
                    gigaPartitionService.Write(request.PartitionId, request.ObjectId, request.Value);
                reply.Ok = ok;
                if (masterServerID != null)
                    reply.MasterServer = masterServerID;
                Console.WriteLine(reply);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Task.FromResult(reply);
        }

        public override Task<ListServerReply> ListServer(ListServerRequest request, ServerCallContext context)
        {
            ListServerReply reply = new ListServerReply();

            reply.Objects.AddRange(gigaPartitionService.ListObjects(request.ServerId).Select(objectId =>
                new PartitionObjectID
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

            reply.Objects.AddRange(gigaPartitionService.ListGlobal().Select(objectId => new PartitionObjectID
                {PartitionId = objectId.PartitionName, ObjectId = objectId.ObjectName}));

            return Task.FromResult(reply);
        }
    }
}