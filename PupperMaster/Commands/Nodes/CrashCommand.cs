using GIGAPuppetMasterProto;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster.Scripts.Commands
{
    internal class CrashCommand : ICommand
    {
        public string Name => "Crash";

        public string Syntax => "server_id";

        public string Description => "Force a process to crash";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            var url = PuppetMaster.ServerMap[Args[0]].Url;

            var channel = GrpcChannel.ForAddress(url);
            var client = new GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

            var reply = client.CrashServerService(new CrashServerRequest());

            PuppetMaster.ServerMap.Remove(Args[0]);
        }
    }
}