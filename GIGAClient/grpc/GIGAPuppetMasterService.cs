using GIGAClient.Commands;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GIGAClient.grpc
{
    class GIGAPuppetMasterService : GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceBase
    {
        private services.GIGAPuppetMasterService gigaPuppetMasterService;

    
        public GIGAPuppetMasterService(services.GIGAPuppetMasterService gigaPuppetMasterService)
        {
            this.gigaPuppetMasterService = gigaPuppetMasterService;
        }

        public override Task<GIGAPuppetMasterProto.StatusReply> StatusService(GIGAPuppetMasterProto.StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GIGAPuppetMasterProto.StatusReply { Ok = gigaPuppetMasterService.Status() });
        }


        public override Task<GIGAPuppetMasterProto.PartitionReply> PartitionService(GIGAPuppetMasterProto.PartitionRequest request, ServerCallContext context)
        {
            int replicationFactor = request.ReplicationFactor;
            string partitionName = request.Id;
            string servers = request.Servers;

            return Task.FromResult(new GIGAPuppetMasterProto.PartitionReply { Ok = gigaPuppetMasterService.Partition(replicationFactor, partitionName, servers) });
        }

       
    }
}
