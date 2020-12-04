using GIGAPuppetMasterProto;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster.Scripts.Commands
{
    internal class FreezeCommand : ICommand
    {
        public string Name => "Freeze";

        public string Syntax => "server_id";

        public string Description => "Simulate a delay in the process";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            var url = PuppetMaster.ServerMap[Args[0]].Url;

            var channel = GrpcChannel.ForAddress(url);
            var client = new GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

            var reply = client.FreezeServerService(new FreezeServerRequest());
        }
    }
}