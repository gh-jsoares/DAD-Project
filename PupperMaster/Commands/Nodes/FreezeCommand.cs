using Grpc.Net.Client;
using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class FreezeCommand : ICommand
    {
        public string Name => "Freeze";

        public string Syntax => "server_id";

        public string Description => "Simulate a delay in the process";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            string url = PuppetMaster.ServerMap[Args[0]].Url;

            GrpcChannel channel = GrpcChannel.ForAddress(url);
            GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient client = new GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

            GIGAPuppetMasterProto.FreezeServerReply reply = client.FreezeServerService(new GIGAPuppetMasterProto.FreezeServerRequest());
        }
    }
}
