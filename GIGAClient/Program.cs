using GIGAClient.Commands;
using GIGAClient.Scripts.Commands;
using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GIGAClient
{
    class Program
    {
        private static CommandExecutor commands;

        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //When client is initiated with a PuppetMaster command
            if (args.Length == 3)
            {
                string name = args[0];
                string url = args[1];
                string file = args[2];


                Console.WriteLine($"Client created with arguments: {args[0]} {args[1]} {args[2]}");
                services.GIGAClientService gigaClientService = new services.GIGAClientService(name, url, file);
                services.GIGAPuppetMasterService gigaPuppetMasterService = new services.GIGAPuppetMasterService(gigaClientService);
                commands = new CommandExecutor(gigaClientService);

                Uri uri = new Uri(url);

                Server server = new Server
                {
                    Services = { GIGAPuppetMasterProto.GIGAPuppetMasterService.BindService(new grpc.GIGAPuppetMasterService(gigaPuppetMasterService)) },
                    Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
                };

                server.Start();
                Console.WriteLine("Client \"Puppet Master Server\" listening on url {0}", uri.AbsoluteUri);


                //Ler script
                try
                {
                    string[] scriptCommands = System.IO.File.ReadAllLines(@"..\..\..\..\GIGAClient\files\scripts\" + file);

                    foreach (string s in scriptCommands)
                    {
                        Console.WriteLine(s);
                        commands.Run(s);
                    }
                }
                catch(Exception e)
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
