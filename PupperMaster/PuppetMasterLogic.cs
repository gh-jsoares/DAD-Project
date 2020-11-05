using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using GIGAPuppetMaster.domain;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster
{
    class PuppetMasterLogic
    {
        private CommandExecutor commands;
        public Dictionary<string, GIGAServerObject> ServerMap { get; } = new Dictionary<string, GIGAServerObject>();
        public Dictionary<string, GIGAPartitionObject> PartitionMap { get; } = new Dictionary<string, GIGAPartitionObject>();
        public Dictionary<string, GIGAPartitionObject> IncompletePartitionMap { get; } = new Dictionary<string, GIGAPartitionObject>();
        public Dictionary<string, string> ClientMap { get; set; } = new Dictionary<string, string>();
        public int ReplicationFactor { get; set; }

        public PuppetMasterLogic()
        {

            commands = new CommandExecutor();

        }

        public string SendCommand(string commandText)
        {
            return commands.Run(commandText, this);
        
        }

        internal void UpdateIncompletePartitionMap(GIGAServerObject serverObject)
        {
            foreach (GIGAPartitionObject partition in IncompletePartitionMap.Values)
            {
                if (partition.Servers.ContainsKey(serverObject.Name))
                {
                    partition.AddServer(serverObject);
                }
            }

            foreach(var partition in IncompletePartitionMap.Values.Where(partition => partition.CanSetup()))
            {
                AddPartition(partition);
                IncompletePartitionMap.Remove(partition.Name);
            }
        }

        internal void AddClient(string username, string url)
        {
            ClientMap.Add(username, url);

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            foreach (GIGAPartitionObject partition in PartitionMap.Values)
            {
                SendPartition(url, partition);
            }
        }

        internal void AddPartition(GIGAPartitionObject partition)
        {
            if(partition.CanSetup())
            {
                PartitionMap.Add(partition.Name, partition);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

                foreach (KeyValuePair<string, string> entry in ClientMap)
                {
                    SendPartition(entry.Value, partition);
                }

                foreach (var entry in partition.Servers.Values)
                {
                    SendPartition(entry.Url, partition);
                }
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

            foreach (GIGAPartitionObject partition in PartitionMap.Values.Where(p => p.Servers.ContainsKey(serverObject.Name)))
            {
                SendPartition(serverObject.Url, partition);
            }
        }

        internal void SendPartition(string url, GIGAPartitionObject partition)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient client = new GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);
            GIGAServerObject[] servers = partition.Servers.Values.ToArray();

            GIGAPuppetMasterProto.PartitionRequest request = new GIGAPuppetMasterProto.PartitionRequest
            {
                PartitionId = partition.Name,
                ReplicationFactor = partition.ReplicationFactor,
            };
            request.Servers.AddRange(servers.Select(server => new GIGAPuppetMasterProto.ServerObject { Id = server.Name, Url = server.Url }));

            client.PartitionService(request);
        }
    }
}