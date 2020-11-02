using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            Uri uri = new Uri(url);

            Grpc.Core.Server server = new Grpc.Core.Server                              //Change Server project namespace
            {
                Services = { CommandListener.BindService(new ClientService(this)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.WriteLine($"Client server listening on host {uri.Host} and port {uri.Port} ");
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
