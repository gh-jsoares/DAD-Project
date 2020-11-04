using GIGAPuppetMaster.domain;
using Grpc.Net.Client;
using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class ServerCommand : ICommand
    {
        public string Name => "Server";

        public string Syntax => "server_id URL min_delay max_delay";

        public string Description => "This command creates a server process identified by server_id, available at URL that delays any incoming message for a random amount of time(specified in milliseconds) between min_delay and max_delay";

        public int NumArgs => 4;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            //Start new server process
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\..\GIGAServer\bin\Debug\netcoreapp3.1\GIGAServer.exe";

            foreach (string arg in Args)
            {
                startInfo.Arguments += $"{arg} ";
            }

            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(startInfo);

            //Add server to PuppetMaster Dictionary
            GIGAServerObject serverObject = new GIGAServerObject(Args[0], Args[1]);
            PuppetMaster.ServerMap.Add(serverObject.Name, serverObject);

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            foreach (KeyValuePair<string, GIGAPartitionObject> entry in PuppetMaster.PartitionMap)
            {
                GrpcChannel channel = GrpcChannel.ForAddress(string.Format("http://{0}:{1}", Args[0], Args[1]));
                GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient client = new GIGAPuppetMasterProto.GIGAPuppetMasterService.GIGAPuppetMasterServiceClient(channel);
                GIGAServerObject[] servers = entry.Value.Servers.Values.ToArray();

                GIGAPuppetMasterProto.PartitionRequest request = new GIGAPuppetMasterProto.PartitionRequest
                {
                    PartitionId = entry.Value.Name,
                    ReplicationFactor = entry.Value.ReplicationFactor,
                };
                request.Servers.AddRange(servers.Select(server => new GIGAPuppetMasterProto.ServerObject { Id = server.Name, Url = server.Url }));

                client.PartitionService(request);
            }
        }
    }
}
