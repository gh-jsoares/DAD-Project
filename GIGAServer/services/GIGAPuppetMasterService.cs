using GIGAPuppetMasterProto;
using GIGAServer.domain;
using GIGAServer.logic;
using System;
using System.Collections.Generic;

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

        public bool Partition(int replicationFactor, string partitionName, string servers)
        {
            throw new NotImplementedException();
        }

        public bool Status()
        {
            return gigaServerService.ShowStatus();
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
