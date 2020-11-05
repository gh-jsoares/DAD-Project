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

        public GIGAServerObject Master { get { return partition.MasterServer; } }

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

        public GIGAObject Read(string name)
        {
            return partition.Read(name);
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
            // TODO MISSING LOCK
            partition.Write(name, value);
        }

        internal bool IsMaster(GIGAServerObject server)
        {
            return server.Name == Master.Name;
        }
    }
}
