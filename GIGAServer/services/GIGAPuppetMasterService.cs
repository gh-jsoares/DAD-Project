using System;
using GIGAServer.domain;

namespace GIGAServer.services
{
    internal class GIGAPuppetMasterService
    {
        private readonly GIGAPartitionService gigaPartitionService;
        private readonly GIGAServerService gigaServerService;

        public GIGAPuppetMasterService(GIGAServerService gigaServerService, GIGAPartitionService gigaPartitionService)
        {
            this.gigaServerService = gigaServerService;
            this.gigaPartitionService = gigaPartitionService;
        }

        public bool Partition(int replicationFactor, string partitionName, GIGAServerObject[] servers)
        {
            return gigaPartitionService.RegisterPartition(partitionName, replicationFactor, servers);
        }

        public bool Status()
        {
            try
            {
                gigaServerService.ShowStatus();
                gigaPartitionService.ShowStatus();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }

        public bool Freeze()
        {
            return gigaServerService.Freeze();
        }

        public bool Unfreeze()
        {
            return gigaServerService.Unfreeze();
        }

        public bool Crash()
        {
            return gigaServerService.Crash();
        }
    }
}