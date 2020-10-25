using ProcessCreationService.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessCreationService.Scripts.Commands
{
    class WaitCommand : ICommand
    {
        public string Name => "Wait";

        public string Syntax => "x_ms";

        public string Description => "Instructs the PuppetMaster to sleep for x_ms milliseconds before reading and executing the next command in the script file";

        public int NumArgs => 3;

        void ICommand.SafeExecute(params string[] Args)
        {
            Console.WriteLine(Args.Length);
        }
    }
}
