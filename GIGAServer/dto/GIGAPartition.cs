﻿using GIGAServer.domain;
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
                partitionMap.Add(server.Name, new GIGAPartitionProto.GIGAPartitionService.GIGAPartitionServiceClient(GrpcChannel.ForAddress(server.Url)));
            }
        }

        internal void ShowStatus()
        {
            partition.ShowStatus();
        }

        internal List<GIGAPartitionObjectID> GetPartitionObjectIDList()
        {
            return partition.GetPartitionObjectIDList();
        }

        internal bool HasServer(string serverId)
        {
            return partition.HasServer(serverId);
        }

        internal void Write(string name, string value)
        {
            Console.WriteLine("name: {0}, value: {1}", name, value);
            partition.Write(name, value);
        }
    }
}
