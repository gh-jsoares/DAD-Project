using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class ReplicationFactorCommand : ICommand
    {
        public string Name => "ReplicationFactor";

        public string Syntax => "r";

        public string Description => "Configures the system to replicate partitions on r servers";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            // TODO validate is int
            PuppetMaster.ReplicationFactor = int.Parse(Args[0]);
        }
    }
}
