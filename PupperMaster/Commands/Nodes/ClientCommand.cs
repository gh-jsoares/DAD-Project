using Grpc.Net.Client;
using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PuppetMaster.Scripts.Commands
{
    class ClientCommand : ICommand
    {
        public string Name => "Client";

        public string Syntax => "username client_URL script_file";

        public string Description => "This command creates a client process identified by the string username, available at client_URL and that will execute the commands in the script file script_file";

        public int NumArgs => 3;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            //Create process with given arguments
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\..\GIGAClient\bin\Debug\netcoreapp3.1\GIGAClient.exe";
            foreach (string arg in Args)
            {
                startInfo.Arguments += $"{arg} ";
            }
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(startInfo);


            //Add client to PuppetMaster Dictionary
            PuppetMaster.ClientMap.Add(Args[0], Args[1]);


            //Send Partitions to client
            GrpcChannel channel = GrpcChannel.ForAddress(Args[1]);
            GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient client = new GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);

            //foreach (KeyValuePair < string, string[]> entry in PuppetMaster.PartitionMap)
            //{
            //    //GIGAPuppetMasterProto.PartitionReply reply = client.PartitionService(new GIGAPuppetMasterProto.PartitionRequest
            //    //{
            //    //    Id = entry.Key,
            //    //    Servers = string.Join(" ", entry.Value)  
            //    //});
            //}

        }
    }
}
