using GIGAServer.services;
using Grpc.Core;
using System;

namespace GIGAServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 1001;
            const string hostname = "localhost";
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
