using GIGAClient.Commands;
using GIGAClient.domain;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string partitionName = request.PartitionId;
            GIGAServerObject[] servers = request.Servers.Select(server => new GIGAServerObject(server.Id, server.Url)).ToArray();

            return Task.FromResult(new GIGAPuppetMasterProto.PartitionReply { Ok = gigaPuppetMasterService.Partition(replicationFactor, partitionName, servers) });
        }

       
    }
}
