using GIGAClient.domain;
using GIGAServerProto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GIGAClient.services
{
    class GIGAClientService
    {
        private GIGAClientObject gigaClientObject;

        public GIGAClientService(string name, string url, string file)
        {
            gigaClientObject = new GIGAClientObject(name, url, file);
        }

        public void read(string partitionId, string objectId, string serverId)
        {
            //TODO: No caso de ja estar attached como aceder ao channel?
            if(gigaClientObject.AttachedServer == null)
            {
                string url = gigaClientObject.ServerMap[serverId];
                GrpcChannel channel = GrpcChannel.ForAddress(url);
                GIGAServerService.GIGAServerServiceClient client = new GIGAServerService.GIGAServerServiceClient(channel);
                client.Read(new ReadRequest { ObjectId = objectId, PartitionId = partitionId });
            }
        }

        internal bool RegisterPartition(string partitionName, int replicationFactor, GIGAServerObject[] servers)
        {
            if (gigaClientObject.PartitionMap.ContainsKey(partitionName)) return false;

            gigaClientObject.PartitionMap.Add(partitionName, new GIGAPartitionObject(partitionName, replicationFactor, servers));

            return true;
        }

        public void write(string partitionId, string objectId, String value)
        {
            //TODO: locking mechanism
            string server_id = gigaClientObject.PartitionMap[partitionId].Servers.Values.First().Name;
            string url = gigaClientObject.ServerMap[server_id];
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            GIGAServerService.GIGAServerServiceClient client = new GIGAServerService.GIGAServerServiceClient(channel);
            client.Write(new WriteRequest { PartitionId = partitionId, ObjectId = objectId, Value = value});
        }

        public void listServer(string serverId)
        {
            GIGAServerObject server = null;
            // TODO CHANGE TO SERVER MAP
            foreach (GIGAPartitionObject partition in gigaClientObject.PartitionMap.Values)
            {
                if (partition.Servers.ContainsKey(serverId))
                {
                    server = partition.Servers[serverId];
                    break;
                }
            }

            if (server == null)
                throw new Exception(string.Format("Server with id: \"{0}\" not found in storage", serverId));

            GrpcChannel channel = GrpcChannel.ForAddress(server.Url);
            GIGAServerService.GIGAServerServiceClient client = new GIGAServerService.GIGAServerServiceClient(channel);
            client.ListServer(new ListServerRequest { ServerId = server.Name });
        }

        internal bool ShowStatus()
        {
            gigaClientObject.ShowStatus();
            return true;
        }

        public void listGlobal()
        {
            GIGAServerObject server = gigaClientObject.PartitionMap.Values.First().Servers.Values.First();
            GrpcChannel channel = GrpcChannel.ForAddress(server.Url);
            GIGAServerService.GIGAServerServiceClient client = new GIGAServerService.GIGAServerServiceClient(channel);
            ListGlobalReply reply = client.ListGlobal(new ListGlobalRequest());
            Console.WriteLine("List of entries: Partition => Object");
            foreach (var item in reply.Objects)
            {
                Console.WriteLine("{0} => {1} | Is Master: {2}", item.PartitionId, item.ObjectId, item.IsMaster);
            }
        }

        public void wait(int time)
        {
            throw new NotImplementedException();
        }
        
    
    }
}
