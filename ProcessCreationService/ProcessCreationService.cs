using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using ProcessCreationService.Commands;

namespace ProcessCreationService
{
    public class ProcessCreationService : CommandListener.CommandListenerBase
    {
        private static CommandExecutor commands;

        public ProcessCreationService()
        {
            commands = new CommandExecutor();
        }

        public override Task<CommandReply> ReceiveCommand(
            CommandRequest request, ServerCallContext context)
        {
            return Task.FromResult(RcvCom(request));
        }

        public CommandReply RcvCom(CommandRequest request)
        {
            String commandText = request.Command;

            Console.WriteLine($"Received new command request: {commandText}");

            return commands.Run(commandText);
        }

    }
}
