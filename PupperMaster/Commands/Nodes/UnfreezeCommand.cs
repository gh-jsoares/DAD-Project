using GIGAPuppetMasterProto;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster.Scripts.Commands
{
    internal class UnfreezeCommand : ICommand
    {
        public string Name => "Unfreeze";

        public string Syntax => "server_id";

        public string Description => "Put a process back to normal operation";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            var url = PuppetMaster.ServerMap[Args[0]].Url;

            var channel = GrpcChannel.ForAddress(url);
            var client = new GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

            var reply = client.UnfreezeServerService(new UnfreezeServerRequest());
        }
    }
}