using PuppetMaster.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class PartitionCommand : ICommand
    {
        public string Name => "Partition";

        public string Syntax => "r partition_name server_id_1 ... server_id_r";

        public string Description => "Configures the system to store r replicas of partition partition_name on the servers identified with the server_ids server_id_1 to serverd_id_r";

        public int NumArgs => 3;   //Pelo menos 2, podem ser mais

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            ArrayList arrayServer = new ArrayList();

            for(int i = 2; i < Args.Length; i++)
            {
                arrayServer.Add(Args);
            }

            PuppetMaster.PartitionMap.Add(Args[1], (string[])arrayServer.ToArray());

        }
    }
}
