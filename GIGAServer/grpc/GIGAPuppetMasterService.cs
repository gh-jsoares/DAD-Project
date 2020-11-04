using GIGAPuppetMasterProto;
using GIGAServer.domain;
using Grpc.Core;
using System.Linq;
using System.Threading.Tasks;

namespace GIGAServer.grpc
{
    class GIGAPuppetMasterService : GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceBase
    {
        private services.GIGAPuppetMasterService gigaPuppetMasterService;

        public GIGAPuppetMasterService(services.GIGAPuppetMasterService gigaPuppetMasterService)
        {
            this.gigaPuppetMasterService = gigaPuppetMasterService;
        }

        public override Task<PartitionReply> PartitionService(PartitionRequest request, ServerCallContext context)
        {
            int replicationFactor = request.ReplicationFactor;
            string partitionName = request.PartitionId;
            GIGAServerObject[] servers = request.Servers.Select(server => new GIGAServerObject(server.Id, server.Url)).ToArray();

            return Task.FromResult(new PartitionReply { Ok = gigaPuppetMasterService.Partition(replicationFactor, partitionName, servers) });
        }

        public override Task<StatusReply> StatusService(StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new StatusReply { Ok = gigaPuppetMasterService.Status() });
        }

        public override Task<UnfreezeServerReply> UnfreezeServerService(UnfreezeServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UnfreezeServerReply { Ok = gigaPuppetMasterService.Unfreeze() });
        }

        public override Task<FreezeServerReply> FreezeServerService(FreezeServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(new FreezeServerReply { Ok = gigaPuppetMasterService.Freeze() });
        }

        public override Task<CrashServerReply> CrashServerService(CrashServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(new CrashServerReply { Ok = gigaPuppetMasterService.Crash() });
        }
    }
}
