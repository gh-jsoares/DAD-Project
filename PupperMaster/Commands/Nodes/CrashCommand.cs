using Grpc.Net.Client;
using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class CrashCommand : ICommand
    {
        public string Name => "Crash";

        public string Syntax => "server_id";

        public string Description => "Force a process to crash";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            string url = PuppetMaster.ServerMap[Args[0]].Url;

            GrpcChannel channel = GrpcChannel.ForAddress(url);
            GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient client = new GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

            GIGAPuppetMasterProto.CrashServerReply reply = client.CrashServerService(new GIGAPuppetMasterProto.CrashServerRequest());

            PuppetMaster.ServerMap.Remove(Args[0]);
        }
    }
}
