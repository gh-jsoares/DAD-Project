using Grpc.Net.Client;
using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class StatusCommand : ICommand
    {
        public string Name => "Status";

        public string Syntax => "";

        public string Description => "Makes all nodes in the system print their current status";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            foreach (var entry in PuppetMaster.ServerMap)
            {
                var server = entry.Value;
                GrpcChannel channel = GrpcChannel.ForAddress(server.Url);
                GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient client = new GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

                GIGAPuppetMasterProto.StatusReply reply = client.StatusService(new GIGAPuppetMasterProto.StatusRequest());
            }
        }
    }
}
