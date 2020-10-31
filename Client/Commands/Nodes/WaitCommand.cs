﻿using Client.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Scripts.Commands
{
    class WaitCommand : ICommand
    {
        public string Name => "wait";

        public string Syntax => "x";

        public string Description => "Delays the execution of the next command for x milliseconds.";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, ClientLogic client)
        {
            throw new NotImplementedException();
        }
    }
}
