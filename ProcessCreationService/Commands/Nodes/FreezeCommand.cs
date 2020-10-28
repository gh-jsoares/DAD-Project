using ProcessCreationService.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessCreationService.Scripts.Commands
{
    class FreezeCommand : ICommand
    {
        public string Name => "Freeze";

        public string Syntax => "server_id";

        public string Description => "Simulate a delay in the process";

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
