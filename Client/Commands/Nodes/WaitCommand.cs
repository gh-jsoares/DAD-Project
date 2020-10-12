using Client.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Scripts.Commands
{
    class WaitCommand : ICommand
    {
        public string Name => "wait";

        public string Syntax => "x";

        public string Description => "Delays the execution of the next command forxmilliseconds.";

        public int NumArgs => 1;

        public void SafeExecute(params string[] Args)
        {
            Console.WriteLine(Args.Length);
        }
    }
}
