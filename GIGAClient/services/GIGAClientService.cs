using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GIGAClient.domain;
using GIGAServerProto;
using Grpc.Net.Client;

namespace GIGAClient.services
{
    internal class GIGAClientService
    {
        private static readonly Random random = new Random();
        private GrpcChannel channel;
        private GIGAServerService.GIGAServerServiceClient client;
        private GIGAPartitionObject currentPartition;
        private GIGAServerObject currentServer;
        private readonly List<string> enabledPartitions;
        private readonly Dictionary<string, string> enabledServers;
        private readonly GIGAClientObject gigaClientObject;

        public GIGAClientService(string name, string url, string file)
        {
            gigaClientObject = new GIGAClientObject(name, url, file);
            enabledServers = new Dictionary<string, string>(gigaClientObject.ServerMap);
            enabledPartitions = new List<string>(gigaClientObject.PartitionMap.Keys);
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

        public void Write(string partitionId, string objectId, string value)
        {
            string masterServer = null;
            bool retry = true;
            do
            {
                try
                {
                    if (!enabledPartitions.Contains(partitionId))
                    {
                        // Server crashed
                        Console.WriteLine($"Requested partition {partitionId} not found. Possibly crashed");
                        return;
                    }

                    if (currentPartition == null || currentPartition.Name != partitionId)
                    {
                        SelectPartition(partitionId);
                    }

                    if (masterServer != null)
                    {
                        SelectServer(masterServer);
                    }

                    if (currentServer == null || !currentPartition.HasServer(currentServer.Name))
                    {
                        SelectRandomServerFromPartition();
                    }

                    AttemptToConnectToCurrentServer();

                    var reply = client.Write(new WriteRequest
                        {PartitionId = partitionId, ObjectId = objectId, Value = value});

                    if (reply.Ok)
                    {
                        Console.WriteLine(
                            $"Object <{partitionId},{objectId}> written in server \"{currentServer.Name}\" with value: {value}");
                        retry = false;
                    }
                    else if (string.IsNullOrEmpty(reply.MasterServer))
                    {
                        // no master server elected yet
                        Console.WriteLine("No master for partition yet. Retrying in 1 second");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        masterServer = reply.MasterServer;
                        Console.WriteLine("Current server is not master.");
                    }
                }
                catch (Exception e)
                {
                    retry = true;

                    if (currentServer != null)
                    {
                        RemoveCurrentPartitionServer();
                    }

                    if (enabledServers.Count == 0)
                    {
                        Console.WriteLine("No servers available.");
                        retry = false;
                    }
                }
                finally
                {
                    channel?.ShutdownAsync()?.Wait();
                }
            } while (retry);
        }

        public void Read(string partitionId, string objectId, string serverHint)
        {
            var nextServer = serverHint;
            var remaining = new List<string>();

            if (currentPartition != null)
                remaining.AddRange(PartitionEnabledServers().Select(s => s.Name));

            var retry = true;
            do
            {
                try
                {
                    if (!enabledPartitions.Contains(partitionId))
                    {
                        // Server crashed
                        Console.WriteLine($"Requested partition {partitionId} not found. Possibly crashed");
                        return;
                    }

                    if (currentPartition == null || currentPartition.Name != partitionId)
                    {
                        SelectPartition(partitionId);
                        remaining.AddRange(PartitionEnabledServers().Select(s => s.Name));
                    }

                    if (currentServer == null || !currentPartition.HasServer(currentServer.Name))
                    {
                        SelectServer(nextServer);
                    }

                    AttemptToConnectToCurrentServer();
                    remaining.Remove(currentServer.Name);

                    var reply = client.Read(new ReadRequest {ObjectId = objectId, PartitionId = partitionId});

                    if (reply.Ok)
                    {
                        Console.WriteLine(
                            $"Object <{partitionId},{objectId}> found in server \"{currentServer.Name}\" with value: {reply.Value}");
                        retry = false;
                    }
                    else if (remaining.Count == 0)
                    {
                        Console.WriteLine($"Object <{partitionId},{objectId}> not found in storage");
                        retry = false;
                    }
                    else
                    {
                        nextServer = remaining.Contains(nextServer) ? serverHint : remaining.First();

                        Console.WriteLine(
                            $"Object <{partitionId},{objectId}> not found in server \"{currentServer.Name}\"");

                        currentServer = null;
                    }
                }
                catch (Exception e)
                {
                    retry = true;

                    if (currentServer != null)
                    {
                        RemoveCurrentPartitionServer();
                    }

                    if (enabledServers.Count == 0)
                    {
                        Console.WriteLine("No servers available.");
                        retry = false;
                    }
                }
                finally
                {
                    channel?.ShutdownAsync()?.Wait();
                }
            } while (retry);
        }

        public void ListServer(string serverId)
        {
            if (!enabledServers.ContainsKey(serverId))
            {
                // Server crashed
                Console.WriteLine($"Requested server {serverId} not found. Possibly crashed");
                return;
            }

            if (currentServer == null || currentServer.Name != serverId)
            {
                currentServer = new GIGAServerObject(serverId, enabledServers[serverId]);
            }

            try
            {
                AttemptToConnectToCurrentServer();

                var reply = client.ListServer(new ListServerRequest {ServerId = currentServer.Name});

                Console.WriteLine("List of entries: Partition => Object");

                foreach (var item in reply.Objects)
                {
                    Console.WriteLine($"{item.PartitionId} => {item.ObjectId} | Is Master: {item.IsMaster.ToString()}");
                }
            }
            catch (Exception e)
            {
                RemoveCurrentPartitionServer();
                // Server crashed
                Console.WriteLine($"Requested server {serverId} not found. Possibly crashed");
            }

            finally
            {
                channel?.ShutdownAsync()?.Wait();
            }
        }

        internal void ShowStatus()
        {
            gigaClientObject.ShowStatus();
        }

        public void ListGlobal()
        {
            bool retry;
            do
            {
                try
                {
                    if (currentServer == null)
                    {
                        SelectRandomServer();
                    }

                    AttemptToConnectToCurrentServer();

                    var reply = client.ListGlobal(new ListGlobalRequest());

                    Console.WriteLine("List of entries: Partition => Object");

                    foreach (var item in reply.Objects)
                        Console.WriteLine("{0} => {1}", item.PartitionId, item.ObjectId);

                    retry = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    retry = true;

                    if (currentServer != null)
                    {
                        RemoveCurrentPartitionServer();
                    }

                    if (enabledServers.Count == 0)
                    {
                        Console.WriteLine("No servers available.");
                        retry = false;
                    }
                }
                finally
                {
                    channel?.ShutdownAsync()?.Wait();
                }
            } while (retry);
        }

        private void AttemptToConnectToCurrentServer()
        {
            Console.WriteLine(
                $"Attempting to connect to server {currentServer.Name} @ {currentServer.Url}");
            channel = GrpcChannel.ForAddress(currentServer.Url);
            client = new GIGAServerService.GIGAServerServiceClient(channel);
            Console.WriteLine($"Connected to server {currentServer.Name} @ {currentServer.Url}");
        }

        private void SelectRandomServerFromPartition()
        {
            var enabledServerObjects = PartitionEnabledServers();

            var index = random.Next(enabledServerObjects.Count);
            currentServer = enabledServerObjects[index];
        }

        private void SelectPartition(string partitionId)
        {
            currentPartition = gigaClientObject.PartitionMap[partitionId];
        }

        private void SelectServer(string serverId)
        {
            if (enabledServers.ContainsKey(serverId))
                currentServer = currentPartition.Servers[serverId];
        }

        private void SelectRandomServer()
        {
            var index = random.Next(enabledServers.Count);
            var name = enabledServers.Keys.ElementAt(index);
            var url = enabledServers[name];
            currentServer = new GIGAServerObject(name, url);
        }

        private List<GIGAServerObject> PartitionEnabledServers()
        {
            return currentPartition.Servers.Values.ToList()
                .FindAll(s => enabledServers.ContainsKey(s.Name));
        }

        private void RemoveCurrentPartitionServer()
        {
            // If current partition only has the current server (which just died)
            if (currentPartition.Servers.All(s => s.Key == currentServer.Name))
            {
                enabledPartitions.Remove(currentPartition.Name);
                currentPartition = null;
            }

            Console.WriteLine(
                $"Server {currentServer.Name} @ {currentServer.Url} is not responding. Retrying with different server...");
            enabledServers.Remove(currentServer.Name);
            currentServer = null;
        }

        public static void Wait(int time)
        {
            Thread.Sleep(time);
        }
    }
}