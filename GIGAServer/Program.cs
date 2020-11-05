using Grpc.Core;
using System;

namespace GIGAServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 1001;
            string hostname = "localhost";
            int minDelay = 0;
            int maxDelay = 0;
            string id = "0";

            if(args.Length == 4)
            {
                id = args[0];

                Uri uri = new Uri(args[1]);

                port = uri.Port;
                hostname = uri.Host;

                minDelay = int.Parse(args[2]);
                maxDelay = int.Parse(args[3]);
            }

            ServerPort serverPort;

            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);

            services.GIGAServerService gigaServerService = new services.GIGAServerService(id, hostname, port, minDelay, maxDelay);
            services.GIGAPartitionService gigaPartitionService = new services.GIGAPartitionService();
            services.GIGAPuppetMasterService gigaPuppetMasterService = new services.GIGAPuppetMasterService(gigaServerService, gigaPartitionService);

            Server server = new Server
            {
                Services = {
                    GIGAServerProto.GIGAServerService.BindService(new grpc.GIGAServerService(gigaServerService)),
                    GIGAPuppetMasterProto.GIGAPuppetMasterService.BindService(new grpc.GIGAPuppetMasterService(gigaPuppetMasterService)),
                    GIGAPartitionProto.GIGAPartitionService.BindService(new grpc.GIGAPartitionService(gigaPartitionService))
                },
                Ports = { serverPort }
            };

            server.Start();


            Console.WriteLine("Insecure GIGAStore server listening on port " + port);
            //Configuring HTTP for client connections in Register method
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            while (true) ; // doesnt exit
        }
    }
}
