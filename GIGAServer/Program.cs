using System;
using GIGAServer.services;
using Grpc.Core;

namespace GIGAServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var port = 1001;
            var hostname = "localhost";
            var minDelay = 0;
            var maxDelay = 0;
            var id = "0";

            if (args.Length == 4)
            {
                id = args[0];

                var uri = new Uri(args[1]);

                port = uri.Port;
                hostname = uri.Host;

                minDelay = int.Parse(args[2]);
                maxDelay = int.Parse(args[3]);
            }

            ServerPort serverPort;

            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);

            var gigaServerService = new GIGAServerService(id, hostname, port, minDelay, maxDelay);
            var gigaPartitionService = new GIGAPartitionService(gigaServerService);
            var gigaPuppetMasterService = new GIGAPuppetMasterService(gigaServerService, gigaPartitionService);


            var server = new Server
            {
                Services =
                {
                    GIGAServerProto.GIGAServerService.BindService(
                        new grpc.GIGAServerService(gigaServerService, gigaPartitionService)),
                    GIGAPuppetMasterProto.GIGAPuppetMasterService.BindService(
                        new grpc.GIGAPuppetMasterService(gigaPuppetMasterService)),
                    GIGAPartitionProto.GIGAPartitionService.BindService(
                        new grpc.GIGAPartitionService(gigaPartitionService))
                },
                Ports = {serverPort}
            };

            server.Start();


            Console.WriteLine("Insecure GIGAStore server listening on port " + port);
            //Configuring HTTP for client connections in Register method
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //Start Raft after everything is setup
            var gigaRaftService = new GIGARaftService(gigaPartitionService, gigaServerService);

            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}