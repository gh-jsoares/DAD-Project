using GIGAClient.domain;

namespace GIGAClient.services
{
    internal class GIGAPuppetMasterService
    {
        private readonly GIGAClientService gigaClientService;

        public GIGAPuppetMasterService(GIGAClientService GIGAClientService)
        {
            gigaClientService = GIGAClientService;
        }

        public bool Partition(int replicationFactor, string partitionName, GIGAServerObject[] servers)
        {
            return gigaClientService.RegisterPartition(partitionName, replicationFactor, servers);
        }

        public void Status()
        {
            gigaClientService.ShowStatus();
        }
    }
}