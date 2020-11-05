using GIGAServer.domain;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;

namespace GIGAServer.dto
{
    class GIGAPartition
    {
        private GIGAPartitionObject partition;
        private Dictionary<string, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient> partitionMap =
            new Dictionary<string, GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient>();

        public GIGAPartition(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            partition = new GIGAPartitionObject(name, replicationFactor, servers);
            foreach (GIGAServerObject server in servers)
            {
                partitionMap.Add(name, new GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient(GrpcChannel.ForAddress(server.Url)));
            }
        }

        internal void ShowStatus()
        {
            partition.ShowStatus();
        }
    }
}
