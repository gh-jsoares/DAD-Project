using System;
using GIGAPuppetMasterProto;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster.Scripts.Commands
{
    internal class StatusCommand : ICommand
    {
        public string Name => "Status";

        public string Syntax => "";

        public string Description => "Makes all nodes in the system print their current status";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            foreach (var entry in PuppetMaster.ClientMap) SendStatusRequest(entry.Value);

            foreach (var entry in PuppetMaster.ServerMap) SendStatusRequest(entry.Value.Url);
        }

        private void SendStatusRequest(string url)
        {
            var channel = GrpcChannel.ForAddress(url);
            var client = new GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

            var reply = client.StatusService(new StatusRequest());
        }
    }
}