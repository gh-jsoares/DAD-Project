using GIGAPuppetMaster.domain;
using Grpc.Core;
using Grpc.Net.Client;
using PuppetMaster.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            string name = Args[1];
            int replicationFactor = int.Parse(Args[0]);
            List<string> serverList = new List<string>(Args);
            serverList.RemoveRange(0, 2);

            GIGAServerObject[] currentServers = PuppetMaster.ServerMap.Values.Where(server => serverList.Contains(server.Name)).ToArray();

            GIGAPartitionObject partitionObject = new GIGAPartitionObject(name, replicationFactor, serverList.ToArray(), currentServers);

            PuppetMaster.AddPartition(partitionObject);
        }
    }
}
