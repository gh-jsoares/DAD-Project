using System;
using System.Collections.Generic;
using System.Linq;
using GIGAPuppetMaster.domain;
using GIGAPuppetMasterProto;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster
{
    internal class PuppetMasterLogic
    {
        private readonly CommandExecutor commands;

        public PuppetMasterLogic()
        {
            commands = new CommandExecutor();
        }

        public Dictionary<string, GIGAServerObject> ServerMap { get; } = new Dictionary<string, GIGAServerObject>();

        public Dictionary<string, GIGAPartitionObject> PartitionMap { get; } =
            new Dictionary<string, GIGAPartitionObject>();

        public Dictionary<string, GIGAPartitionObject> IncompletePartitionMap { get; } =
            new Dictionary<string, GIGAPartitionObject>();

        public Dictionary<string, string> ClientMap { get; set; } = new Dictionary<string, string>();
        public int ReplicationFactor { get; set; }

        public string SendCommand(string commandText)
        {
            return commands.Run(commandText, this);
        }

        internal void UpdateIncompletePartitionMap(GIGAServerObject serverObject)
        {
            foreach (var partition in IncompletePartitionMap.Values)
                if (partition.Servers.ContainsKey(serverObject.Name))
                    partition.AddServer(serverObject);

            foreach (var partition in IncompletePartitionMap.Values.Where(partition => partition.CanSetup()))
            {
                AddPartition(partition);
                IncompletePartitionMap.Remove(partition.Name);
            }
        }

        internal void AddClient(string username, string url)
        {
            ClientMap.Add(username, url);

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            foreach (var partition in PartitionMap.Values) SendPartition(url, partition);
        }

        internal void AddPartition(GIGAPartitionObject partition)
        {
            if (partition.CanSetup())
            {
                PartitionMap.Add(partition.Name, partition);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

                foreach (var entry in ClientMap) SendPartition(entry.Value, partition);

                foreach (var entry in ServerMap.Values) SendPartition(entry.Url, partition);
            }
            else
            {
                IncompletePartitionMap.Add(partition.Name, partition);
            }
        }

        internal void AddServer(GIGAServerObject serverObject)
        {
            ServerMap.Add(serverObject.Name, serverObject);

            UpdateIncompletePartitionMap(serverObject);

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            foreach (var partition in PartitionMap.Values) SendPartition(serverObject.Url, partition);
        }

        internal void SendPartition(string url, GIGAPartitionObject partition)
        {
            var channel = GrpcChannel.ForAddress(url);
            var client = new GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);
            var servers = partition.Servers.Values.ToArray();

            var request = new PartitionRequest
            {
                PartitionId = partition.Name,
                ReplicationFactor = partition.ReplicationFactor
            };
            request.Servers.AddRange(servers.Select(server => new ServerObject {Id = server.Name, Url = server.Url}));

            client.PartitionService(request);
        }
    }
}