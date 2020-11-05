using GIGAPuppetMasterProto;
using GIGAServer.domain;
using GIGAServer.dto;
using System;
using System.Reflection;

namespace GIGAServer.services
{
    class GIGAPuppetMasterService
    {
        private GIGAServerService gigaServerService;
        private GIGAPartitionService gigaPartitionService;

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
            gigaServerService.ShowStatus();
            gigaPartitionService.ShowStatus();

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
