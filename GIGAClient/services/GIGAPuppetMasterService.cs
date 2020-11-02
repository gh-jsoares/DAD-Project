using GIGAClient.Commands;
using Grpc.Core;
using PuppetMaster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GIGAClient.services
{
    class GIGAPuppetMasterService : GIGAPuppetMasterProtoService.GIGAPuppetMasterProtoServiceBase
    {
        private ClientLogic clientLogic;

        public GIGAPuppetMasterService(ClientLogic clientLogic)
        {
            this.clientLogic = clientLogic;
        }

        public override Task<StatusReply> StatusService(StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(HandleStatusService(request));
        }

        public StatusReply HandleStatusService(StatusRequest request)
        {

            Console.WriteLine($"Client {clientLogic.Username} is up in URL {clientLogic.Url}");

            return new StatusReply
            {
                Ok = true
            };
        }


        public override Task<PartitionReply> PartitionService(PartitionRequest request, ServerCallContext context)
        {
            return Task.FromResult(HandlePartitionService(request));
        }

        public PartitionReply HandlePartitionService(PartitionRequest request)
        {

            string[] arrayServer = request.Servers.Split(" ");

            clientLogic.PartitionMap.Add(request.Id, arrayServer);

            Console.WriteLine($"Added new Partition {request.Id} with {arrayServer.Length} servers: {request.Servers}");


            return new PartitionReply
            {
                Ok = true
            };
        }


        public override Task<ServerReply> ServerService(ServerRequest request, ServerCallContext context)
        {
            return Task.FromResult(HandleServerService(request));
        }

        public ServerReply HandleServerService(ServerRequest request)
        {

            clientLogic.ServerMap.Add(request.Id, request.Url);

            Console.WriteLine($"Added new server {request.Id} with URL {request.Url}");


            return new ServerReply
            {
                Ok = true
            };
        }
    }
}
