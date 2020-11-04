using GIGAClient.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAClient.Scripts.Commands
{
    class WaitCommand : ICommand
    {
        public string Name => "wait";

        public string Syntax => "x";

        public string Description => "Delays the execution of the next command for x milliseconds.";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, services.GIGAClientService service)
        {
            throw new NotImplementedException();
        }
    }
}
