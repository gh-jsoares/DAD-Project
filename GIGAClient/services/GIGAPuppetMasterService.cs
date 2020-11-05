﻿using GIGAClient.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAClient.services
{
    class GIGAPuppetMasterService
    {
        private GIGAClientService gigaClientService;

        public GIGAPuppetMasterService(GIGAClientService GIGAClientService)
        {
            this.gigaClientService = GIGAClientService;
        }

        public bool Partition(int replicationFactor, string partitionName, GIGAServerObject[] servers)
        {
            return gigaClientService.RegisterPartition(partitionName, replicationFactor, servers);
        }

        public bool Status()
        {
            return gigaClientService.ShowStatus();
        }
      
    }
}
