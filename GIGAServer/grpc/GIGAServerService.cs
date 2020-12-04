using System;
using System.Linq;
using System.Threading.Tasks;
using GIGAServerProto;
using Grpc.Core;
using Object = GIGAServerProto.Object;

namespace GIGAServer.grpc
{
    internal class GIGAServerService : GIGAServerProto.GIGAServerService.GIGAServerServiceBase
    {
        private readonly services.GIGAPartitionService gigaPartitionService;
        private readonly services.GIGAServerService gigaServerService;

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
                var obj = gigaPartitionService.Read(request.PartitionId, request.ObjectId);

                if (obj != null)
                    reply = new ReadReply
                    {
                        Value = obj.Value, ObjectId = obj.Name, PartitionId = obj.Partition.Name, Ok = true,
                        Timestamp = obj.Timestamp
                    };
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
            var reply = new ListServerReply();

            foreach (var partition in gigaPartitionService.ListPartitions())
                reply.Partitions.Add(new Partition
                {
                    PartitionId = partition.Partition.Name,
                    IsMaster = partition.IsMaster(gigaServerService.Server),
                    Objects =
                    {
                        partition.GetObjects().Select(obj => new Object
                            {
                                ObjectId = obj.Name,
                                Value = obj.Value,
                                Timestamp = obj.Timestamp
                            }
                        ).ToList()
                    }
                });

            return Task.FromResult(reply);
        }
    }
}