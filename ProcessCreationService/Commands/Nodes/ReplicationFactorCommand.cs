using ProcessCreationService.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessCreationService.Scripts.Commands
{
    class ReplicationFactorCommand : ICommand
    {
        public string Name => "ReplicationFactor";

        public string Syntax => "r";

        public string Description => "Configures the system to replicate partitions on r servers";

        public int NumArgs => 1;

        CommandReply ICommand.SafeExecute(params string[] Args)
        {
            Console.WriteLine(Args.Length);

            return new CommandReply
            {
                Ok = true,
                Error = $"{Name} Command sent successfully",
            };
        }
    }
}
