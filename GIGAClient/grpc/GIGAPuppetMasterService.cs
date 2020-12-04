using System.Linq;
using System.Threading.Tasks;
using GIGAClient.domain;
using GIGAPuppetMasterProto;
using Grpc.Core;

namespace GIGAClient.grpc
{
    internal class GIGAPuppetMasterService : GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceBase
    {
        private readonly services.GIGAPuppetMasterService gigaPuppetMasterService;


        public GIGAPuppetMasterService(services.GIGAPuppetMasterService gigaPuppetMasterService)
        {
            this.gigaPuppetMasterService = gigaPuppetMasterService;
        }

        public override Task<StatusReply> StatusService(StatusRequest request, ServerCallContext context)
        {
            gigaPuppetMasterService.Status();
            return Task.FromResult(new StatusReply {Ok = true});
        }


        public override Task<PartitionReply> PartitionService(PartitionRequest request, ServerCallContext context)
        {
            var replicationFactor = request.ReplicationFactor;
            var partitionName = request.PartitionId;
            var servers = request.Servers.Select(server => new GIGAServerObject(server.Id, server.Url)).ToArray();

            return Task.FromResult(new PartitionReply
                {Ok = gigaPuppetMasterService.Partition(replicationFactor, partitionName, servers)});
        }
    }
}