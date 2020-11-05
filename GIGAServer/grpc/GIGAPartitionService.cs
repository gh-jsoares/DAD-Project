using GIGAServerProto;
using Grpc.Core;
using System.Threading.Tasks;

namespace GIGAServer.grpc
{
    class GIGAPartitionService : GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceBase
    {
        private services.GIGAPartitionService gigaPartitionService;

        public GIGAPartitionService(services.GIGAPartitionService gigaPartitionService)
        {
            this.gigaPartitionService = gigaPartitionService;
        }
    }
}
