﻿using ProcessCreationService.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessCreationService.Scripts.Commands
{
    class CrashCommand : ICommand
    {
        public string Name => "Crash";

        public string Syntax => "server_id";

        public string Description => "Force a process to crash";

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
