using GIGAClient.Commands;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GIGAClient.services
{
    class GIGAPuppetMasterService : GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceBase
    {
        private ClientLogic clientLogic;

        public GIGAPuppetMasterService(ClientLogic clientLogic)
        {
            this.clientLogic = clientLogic;
        }

        public override Task<GIGAPuppetMasterProto.StatusReply> StatusService(GIGAPuppetMasterProto.StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(HandleStatusService(request));
        }

        public GIGAPuppetMasterProto.StatusReply HandleStatusService(GIGAPuppetMasterProto.StatusRequest request)
        {

            Console.WriteLine($"Client {clientLogic.Username} is up in URL {clientLogic.Url}");

            return new GIGAPuppetMasterProto.StatusReply
            {
                Ok = true
            };
        }


        public override Task<GIGAPuppetMasterProto.PartitionReply> PartitionService(GIGAPuppetMasterProto.PartitionRequest request, ServerCallContext context)
        {
            return Task.FromResult(HandlePartitionService(request));
        }

        public GIGAPuppetMasterProto.PartitionReply HandlePartitionService(GIGAPuppetMasterProto.PartitionRequest request)
        {

            string[] arrayServer = request.Servers.Split(" ");

            clientLogic.PartitionMap.Add(request.Id, arrayServer);

            Console.WriteLine($"Added new Partition {request.Id} with {arrayServer.Length} servers: {request.Servers}");


            return new GIGAPuppetMasterProto.PartitionReply
            {
                Ok = true
            };
        }
    }
}
