using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class StatusCommand : ICommand
    {
        public string Name => "Status";

        public string Syntax => "";

        public string Description => "Makes all nodes in the system print their current status";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

        }
    }
}
