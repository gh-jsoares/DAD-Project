using GIGAClient.Commands;
using Grpc.Core;
using PuppetMaster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GIGAClient.services
{
    class GIGAPuppetMasterService : GIGAPupperMasterProtoService.GIGAPupperMasterProtoServiceBase
    {
        private CommandExecutor commandExecutor = new CommandExecutor(true);
        private ClientLogic clientLogic;

        public GIGAPuppetMasterService(ClientLogic clientLogic)
        {
            this.clientLogic = clientLogic;
        }

        public override Task<CommandReply> SendCommand(CommandRequest request, ServerCallContext context)
        {
            return Task.FromResult(HandleCommand(request));
        }

        public CommandReply HandleCommand(CommandRequest request)
        {
            Console.WriteLine($"New command received from Puppet Master : {request.Text}");

            commandExecutor.Run(request.Text, clientLogic);

            return new CommandReply
            {
                Ok = true
            };
        }
    }
}
