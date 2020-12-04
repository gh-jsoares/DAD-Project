using GIGAClient.domain;
using GIGAServerProto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GIGAClient.services
{
    class GIGAClientService
    {
        private GIGAClientObject gigaClientObject;
        private GrpcChannel channel;
        private GIGAServerService.GIGAServerServiceClient client;
        private GIGAServerObject currentServer;
        private GIGAPartitionObject currentPartition;
        private Dictionary<string, string> enabledServers;
        private List<string> enabledPartitions;
        private static Random random = new Random();

        public GIGAClientService(string name, string url, string file)
        {
            gigaClientObject = new GIGAClientObject(name, url, file);
            enabledServers = new Dictionary<string, string>(gigaClientObject.ServerMap);
            enabledPartitions = new List<string>(gigaClientObject.PartitionMap.Keys);
        }

        public void ConnectToPartition(string partitionId)
        {
            currentPartition = gigaClientObject.PartitionMap[partitionId];
            currentServer = currentPartition.GetRandomServer();
            Console.WriteLine("Connecting to random server on partition \"{0}\"", partitionId);
            // TODO ADD NULL CHECK
            channel = GrpcChannel.ForAddress(currentServer.Url);
            client = new GIGAServerService.GIGAServerServiceClient(channel);
        }

        public void ConnectToPartitionServer(string partitionId, string serverId)
        {
            Console.WriteLine("Connecting to server \"{0}\" on partition \"{1}\"", serverId, partitionId);
            currentPartition = gigaClientObject.PartitionMap[partitionId];
            currentServer = currentPartition.GetServer(serverId);
            // TODO ADD NULL CHECK
            channel = GrpcChannel.ForAddress(currentServer.Url);
            client = new GIGAServerService.GIGAServerServiceClient(channel);
        }

        internal bool RegisterPartition(string partitionName, int replicationFactor, GIGAServerObject[] servers)
        {
            if (gigaClientObject.PartitionMap.ContainsKey(partitionName)) return false;

            var partition = new GIGAPartitionObject(partitionName, replicationFactor, servers);
            gigaClientObject.PartitionMap.Add(partitionName, partition);

            enabledPartitions.Add(partitionName);
            foreach (var server in servers)
            {
                if (!enabledServers.ContainsKey(server.Name))
                    enabledServers.Add(server.Name, server.Url);
            }

            return true;
        }


        public void Read(string partitionId, string objectId, string serverId)
        {
            currentPartition = null;

            if (!selectPartition(partitionId)) return;

            var remaining = new List<string>(currentPartition.Servers.Keys);
            var nextServer = currentServer.Name;
            remaining.Remove(nextServer);

            var retry = true;
            while (retry)
            {
                while (client == null)
                {
                    try
                    {
                        if (!selectPartition(partitionId, nextServer)) return;
                        Console.WriteLine(
                            $"Attempting to connect to server {currentServer.Name} @ {currentServer.Url}");
                        channel = GrpcChannel.ForAddress(currentServer.Url);
                        client = new GIGAServerService.GIGAServerServiceClient(channel);
                        Console.WriteLine($"Connected to server {currentServer.Name} @ {currentServer.Url}");

                        if (currentServer != null)
                        {
                            var reply = client.Read(new ReadRequest {ObjectId = objectId, PartitionId = partitionId});

                            if (!reply.Ok)
                            {
                                client = null;

                                Console.WriteLine(
                                    $"Object <{partitionId},{objectId}> not found in server \"{currentServer.Name}\"");

                                if (remaining.Count == 0)
                                {
                                    Console.WriteLine(
                                        $"Object <{partitionId},{objectId}> not found in storage");
                                    break;
                                }

                                if (remaining.Contains(serverId))
                                    nextServer = serverId;
                                else
                                {
                                    nextServer = remaining.First();
                                    remaining.Remove(nextServer);
                                }

                                continue;
                            }

                            Console.WriteLine(
                                $"Object <{partitionId},{objectId}> found in server \"{currentServer.Name}\" with value: {reply.Value} and timestamp {reply.Timestamp}");
                        }

                        retry = false;
                    }
                    catch (Exception e)
                    {
                        if (currentServer != null)
                        {
                            Console.WriteLine(
                                $"Server {currentServer.Name} @ {currentServer.Url} is not responding. Retrying with different server...");

                            removeCurrentPartitionServer();
                        }

                        if (enabledServers.Count == 0)
                        {
                            Console.WriteLine($"No servers available.");
                            break;
                        }
                    }
                }

                cleanConnection();
            }
        }


        public void Write(string partitionId, string objectId, string value)
        {
            if (!selectPartition(partitionId)) return;
            string masterServer = null;

            var retry = true;
            while (retry)
            {
                while (client == null)
                {
                    try
                    {
                        if (!selectPartition(partitionId, masterServer)) return;
                        Console.WriteLine(
                            $"Attempting to connect to server {currentServer.Name} @ {currentServer.Url}");
                        channel = GrpcChannel.ForAddress(currentServer.Url);
                        client = new GIGAServerService.GIGAServerServiceClient(channel);
                        Console.WriteLine($"Connected to server {currentServer.Name} @ {currentServer.Url}");

                        if (currentServer != null)
                        {
                            var reply = client.Write(new WriteRequest
                                {PartitionId = partitionId, ObjectId = objectId, Value = value});

                            if (!reply.Ok)
                            {
                                client = null;

                                if (string.IsNullOrEmpty(reply.MasterServer))
                                {
                                    // no master server elected yet
                                    Console.WriteLine("No master for partition yet. Retrying in 1 second");
                                    Thread.Sleep(1000);
                                    continue;
                                }

                                masterServer = reply.MasterServer;
                                Console.WriteLine("Current server is not master.");

                                continue;
                            }

                            Console.WriteLine(
                                $"Object <{partitionId},{objectId}> written in server \"{currentServer.Name}\" with value: {value}");
                        }

                        retry = false;
                    }
                    catch (Exception e)
                    {
                        if (currentServer != null)
                        {
                            Console.WriteLine(
                                $"Server {currentServer.Name} @ {currentServer.Url} is not responding. Retrying with different server...");

                            removeCurrentPartitionServer();
                        }

                        if (enabledServers.Count == 0)
                        {
                            Console.WriteLine($"No servers available.");
                            break;
                        }
                    }
                }

                cleanConnection();
            }
        }

        private bool selectPartition(string partitionId, string serverHint = null)
        {
            currentServer = null;
            currentPartition = null;
            selectRandomPartitionServer(partitionId, serverHint);
            if (currentPartition == null || currentPartition.Name != partitionId)
            {
                Console.WriteLine($"Requested partition {partitionId} has no servers. Possibly crashed");
                return false;
            }

            return true;
        }

        public void ListServer(string serverId)
        {
            currentServer = null;
            selectRandomPartitionServer(serverHint: serverId);
            if (currentServer == null || currentServer.Name != serverId)
            {
                Console.WriteLine($"Requested server {serverId} not found. Possibly crashed");
                return;
            }

            SafeExecute(() =>
            {
                var reply = client.ListServer(new ListServerRequest {ServerId = currentServer.Name});
                Console.WriteLine("List of entries: Partition => Object");
                foreach (var item in reply.Objects)
                {
                    Console.WriteLine($"{item.PartitionId} => {item.ObjectId} | Is Master: {item.IsMaster.ToString()}");
                }
            }, serverId);
        }

        internal void ShowStatus()
        {
            SafeExecute(() => gigaClientObject.ShowStatus());
        }

        public void ListGlobal()
        {
            SafeExecute(() =>
            {
                var reply = client.ListGlobal(new ListGlobalRequest());
                Console.WriteLine("List of entries: Partition => Object");
                foreach (var item in reply.Objects)
                {
                    Console.WriteLine("{0} => {1}", item.PartitionId, item.ObjectId);
                }
            });
        }

        private delegate void ParamsAction();

        private void SafeExecute(ParamsAction action, string serverFallback = null, string partitionFallback = null)
        {
            var retry = true;
            while (retry)
            {
                connectToServer(partitionFallback, serverFallback);

                try
                {
                    if (currentServer != null)
                    {
                        action();
                    }

                    retry = false;
                }
                catch (Exception e) // server crashed
                {
                    Console.WriteLine(
                        $"Server {currentServer.Name} @ {currentServer.Url} is not responding. Retrying with different server...");
                    removeCurrentPartitionServer();
                }

                cleanConnection();
            }
        }

        private void connectToServer(string partition = null, string serverHint = null)
        {
            while (client == null)
            {
                try
                {
                    selectRandomPartitionServer(partition, serverHint);
                    Console.WriteLine($"Attempting to connect to server {currentServer.Name} @ {currentServer.Url}");
                    channel = GrpcChannel.ForAddress(currentServer.Url);
                    client = new GIGAServerService.GIGAServerServiceClient(channel);
                    Console.WriteLine($"Connected to server {currentServer.Name} @ {currentServer.Url}");
                }
                catch (Exception e)
                {
                    if (currentServer != null)
                    {
                        Console.WriteLine(
                            $"Server {currentServer.Name} @ {currentServer.Url} is not responding. Retrying with different server...");

                        removeCurrentPartitionServer();
                    }

                    if (enabledServers.Count == 0)
                    {
                        Console.WriteLine($"No servers available.");
                        break;
                    }
                }
            }
        }

        private void removeCurrentPartitionServer()
        {
            // If current partition only has the current server (which just died)
            if (currentPartition.Servers.All(s => s.Key == currentServer.Name))
            {
                enabledPartitions.Remove(currentPartition.Name);
                currentPartition = null;
            }

            enabledServers.Remove(currentServer.Name);
            currentServer = null;
            client = null;
        }

        private void selectRandomPartitionServer(string partition = null, string serverHint = null)
        {
            if (currentServer == null)
            {
                if (currentPartition == null)
                {
                    var partitionName = partition;

                    if (partition == null || !enabledPartitions.Contains(partition))
                    {
                        var index = random.Next(enabledPartitions.Count);
                        partitionName = enabledPartitions[index];
                    }

                    currentPartition = gigaClientObject.PartitionMap[partitionName];
                }

                var enabledServerObjects = currentPartition.Servers.Values.ToList()
                    .FindAll(s => enabledServers.ContainsKey(s.Name));

                if (serverHint != null && enabledServerObjects.Any(s => s.Name == serverHint))
                    currentServer = enabledServerObjects.Find(s => s.Name == serverHint);
                else // choose randomly
                {
                    var index = random.Next(enabledServerObjects.Count);
                    currentServer = enabledServerObjects[index];
                }
            }
        }

        private void cleanConnection()
        {
            channel?.ShutdownAsync().Wait();
            client = null;
        }

        public void wait(int time)
        {
            Thread.Sleep(time);
        }
    }
}