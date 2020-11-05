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

        public GIGAClientService(string name, string url, string file)
        {
            gigaClientObject = new GIGAClientObject(name, url, file);
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

        public void ConnectToServer(string serverId)
        {
            Console.WriteLine("Connecting to server \"{0}\" on random partition", serverId);
            currentPartition = gigaClientObject.GetRandomPartitionForServer(serverId);
            currentServer = currentPartition.GetServer(serverId);
            channel = GrpcChannel.ForAddress(currentServer.Url);
            client = new GIGAServerService.GIGAServerServiceClient(channel);
        }

        public void ConnectToRandomServer()
        {
            Console.WriteLine("Connecting to random server on random partition");
            currentPartition = gigaClientObject.GetRandomPartition();
            currentServer = currentPartition.GetRandomServer();
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

        public void read(string partitionId, string objectId, string serverId)
        {
            if (currentPartition == null || currentPartition.Name != partitionId) ConnectToPartition(partitionId);
            ReadReply reply = client.Read(new ReadRequest { ObjectId = objectId, PartitionId = partitionId });
            if (!reply.Ok && serverId != "-1")
            {
                Console.WriteLine("Object <{0},{1}> not found in server \"{2}\"", partitionId, objectId, currentServer.Name);
                ConnectToPartitionServer(partitionId, serverId);
                reply = client.Read(new ReadRequest { ObjectId = objectId, PartitionId = partitionId });
            }

            if (reply.Ok)
                Console.WriteLine("Object <{0},{1}> found in server \"{2}\" with value: {3}", partitionId, objectId, currentServer.Name, reply.Value);
            else
                Console.WriteLine("Object <{0},{1}> not found in current server", partitionId, objectId);
        }

        internal bool RegisterPartition(string partitionName, int replicationFactor, GIGAServerObject[] servers)
        {
            if (gigaClientObject.PartitionMap.ContainsKey(partitionName)) return false;

            gigaClientObject.PartitionMap.Add(partitionName, new GIGAPartitionObject(partitionName, replicationFactor, servers));

            return true;
        }

        public void write(string partitionId, string objectId, string value)
        {
            if (currentPartition == null || currentPartition.Name != partitionId) ConnectToPartition(partitionId);
            WriteReply reply = client.Write(new WriteRequest { PartitionId = partitionId, ObjectId = objectId, Value = value});

            if (!reply.Ok)
            {
                Console.WriteLine("Current server is not master.");
                ConnectToPartitionServer(partitionId, reply.MasterServer);
                reply = client.Write(new WriteRequest { PartitionId = partitionId, ObjectId = objectId, Value = value});
            }

            if (reply.Ok)
                Console.WriteLine("Object <{0},{1}> written in server \"{2}\" with value: {3}", partitionId, objectId, currentServer.Name, value);
            else
                Console.WriteLine("An error occurred.");
        }

        public void listServer(string serverId)
        {
            if (currentServer == null || currentServer.Name != serverId) ConnectToServer(serverId);
            ListServerReply reply = client.ListServer(new ListServerRequest { ServerId = currentServer.Name });
            Console.WriteLine("List of entries: Partition => Object");
            foreach (var item in reply.Objects)
            {
                Console.WriteLine("{0} => {1} | Is Master: {2}", item.PartitionId, item.ObjectId, item.IsMaster);
            }
        }

        internal bool ShowStatus()
        {
            gigaClientObject.ShowStatus();
            return true;
        }

        public void listGlobal()
        {
            if (currentServer == null) ConnectToRandomServer();
            ListGlobalReply reply = client.ListGlobal(new ListGlobalRequest());
            Console.WriteLine("List of entries: Partition => Object");
            foreach (var item in reply.Objects)
            {
                Console.WriteLine("{0} => {1}", item.PartitionId, item.ObjectId);
            }
        }

        public void wait(int time)
        {
            Thread.Sleep(time);
        }
        
    
    }
}
