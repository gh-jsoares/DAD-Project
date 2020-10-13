using Client.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Scripts.Commands
{
    class BeginRepeatCommand : ICommand
    {
        public string Name => "begin-repeat";

        public string Syntax => "x";

        public string Description => "Repeats x number of times the commands following this command and before the next end-repeat. No nesting allowed. \"$i\" is replaced with the number of the iteration";

        public int NumArgs => 1;

        void ICommand.SafeExecute(params string[] Args)
        {
            Console.WriteLine(Args.Length);
        }
    }
}
