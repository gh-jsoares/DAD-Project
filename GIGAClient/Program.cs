using System;
using System.IO;
using System.Threading;
using GIGAClient.Commands;
using GIGAClient.services;
using Grpc.Core;

namespace GIGAClient
{
    internal class Program
    {
        private static CommandExecutor commands;

        private static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //When client is initiated with a PuppetMaster command
            if (args.Length == 3)
            {
                var name = args[0];
                var url = args[1];
                var file = args[2];


                Console.WriteLine($"Client created with arguments: {args[0]} {args[1]} {args[2]}");
                var gigaClientService = new GIGAClientService(name, url, file);
                var gigaPuppetMasterService = new GIGAPuppetMasterService(gigaClientService);
                commands = new CommandExecutor(gigaClientService);

                var uri = new Uri(url);

                var server = new Server
                {
                    Services =
                    {
                        GIGAPuppetMasterProto.GIGAPuppetMasterService.BindService(
                            new grpc.GIGAPuppetMasterService(gigaPuppetMasterService))
                    },
                    Ports = {new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure)}
                };

                server.Start();
                Console.WriteLine("Client \"Puppet Master Server\" listening on url {0}", uri.AbsoluteUri);


                //Ler script

                try
                {
                    Console.WriteLine("Waiting for partition to register");
                    var scriptCommands = File.ReadAllLines(@"..\..\..\..\GIGAClient\files\scripts\" + file);

                    Thread.Sleep(2000);

                    foreach (var s in scriptCommands)
                    {
                        Console.WriteLine(s);
                        commands.Run(s);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                string command;

                while (true)
                {
                    Console.Write("> ");
                    command = Console.ReadLine();
                    Console.WriteLine("...");
                    commands.Run(command);
                }
            }

            Console.ReadLine();
        }
    }
}