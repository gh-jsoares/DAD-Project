using System.Linq;
using System.Threading.Tasks;
using GIGAPuppetMasterProto;
using GIGAServer.domain;
using Grpc.Core;

namespace GIGAServer.grpc
{
    internal class GIGAPuppetMasterService : GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceBase
    {
        private readonly services.GIGAPuppetMasterService gigaPuppetMasterService;

        public GIGAPuppetMasterService(services.GIGAPuppetMasterService gigaPuppetMasterService)
        {
            this.gigaPuppetMasterService = gigaPuppetMasterService;
        }

        public override Task<PartitionReply> PartitionService(PartitionRequest request, ServerCallContext context)
        {
            var replicationFactor = request.ReplicationFactor;
            var partitionName = request.PartitionId;
            var servers = request.Servers.Select(server => new GIGAServerObject(server.Id, server.Url)).ToArray();

            return Task.FromResult(new PartitionReply
                {Ok = gigaPuppetMasterService.Partition(replicationFactor, partitionName, servers)});
        }

        public override Task<StatusReply> StatusService(StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new StatusReply {Ok = gigaPuppetMasterService.Status()});
        }

        public override Task<UnfreezeServerReply> UnfreezeServerService(UnfreezeServerRequest request,
            ServerCallContext context)
        {
            return Task.FromResult(new UnfreezeServerReply {Ok = gigaPuppetMasterService.Unfreeze()});
        }

        public override Task<FreezeServerReply> FreezeServerService(FreezeServerRequest request,
            ServerCallContext context)
        {
            return Task.FromResult(new FreezeServerReply {Ok = gigaPuppetMasterService.Freeze()});
        }

        public override Task<CrashServerReply> CrashServerService(CrashServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CrashServerReply {Ok = gigaPuppetMasterService.Crash()});
        }
    }
}