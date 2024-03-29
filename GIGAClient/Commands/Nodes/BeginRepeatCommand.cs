﻿using GIGAClient.Commands;
using GIGAClient.services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAClient.Scripts.Commands
{
    class BeginRepeatCommand : ICommand
    {
        public string Name => "begin-repeat";

        public string Syntax => "x";

        public string Description => "Repeats x number of times the commands following this command and before the next end-repeat. No nesting allowed. \"$i\" is replaced with the number of the iteration";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, GIGAClientService service)
        {
            string v = Args[0];
            // TODO: validate argument as integer
            CommandExecutor.LoopCount = int.Parse(v);
        }
    }
}
