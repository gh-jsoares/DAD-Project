using Grpc.Net.Client;
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
                arrayServer.Add(Args[i]);

            }

            PuppetMaster.PartitionMap.Add(Args[1], (string[])arrayServer.ToArray(typeof(string)));


            //Send Partitions to every client
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            foreach (KeyValuePair<string, string> entry in PuppetMaster.ClientMap)
            {

                GrpcChannel channel = GrpcChannel.ForAddress(entry.Value);
                GIGAPuppetMasterProtoService.GIGAPuppetMasterProtoServiceClient client = new GIGAPuppetMasterProtoService.GIGAPuppetMasterProtoServiceClient(channel);

                PartitionReply reply = client.PartitionService(new PartitionRequest
                {
                    Id = Args[1],
                    Servers =   string.Join(" ", (string[])arrayServer.ToArray(typeof(string))) 
                });

            }

        }
    }
}
