using GIGAServer.services;
using Grpc.Core;
using System;

namespace GIGAServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port;
            string hostname;

            if(args.Length != 4)
            {
                port = 1001;
                hostname = "localhost";
            }
            else
            {
                Uri uri = new Uri(args[1]);

                port = uri.Port;
                hostname = uri.Host;
            }
            string startupMessage;
            ServerPort serverPort;

            serverPort = new ServerPort(hostname, port, ServerCredentials.Insecure);
            startupMessage = "Insecure GIGAStore server listening on port " + port;

            Server server = new Server
            {
                Services = { GIGAServerProtoService.BindService(new GIGAServerService()) },
                Ports = { serverPort }
            };

            server.Start();

            Console.WriteLine(startupMessage);
            //Configuring HTTP for client connections in Register method
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            while (true) ; // doesnt exit
        }
    }
}
