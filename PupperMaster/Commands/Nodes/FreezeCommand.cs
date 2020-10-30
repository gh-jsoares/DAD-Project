using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class FreezeCommand : ICommand
    {
        public string Name => "Freeze";

        public string Syntax => "server_id";

        public string Description => "Simulate a delay in the process";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

        }
    }
}
