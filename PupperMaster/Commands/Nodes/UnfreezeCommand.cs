using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class UnfreezeCommand : ICommand
    {
        public string Name => "Unfreeze";

        public string Syntax => "server_id";

        public string Description => "Put a process back to normal operation";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);


        }
    }
}
