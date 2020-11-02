using GIGAClient.Commands;
using System;
using System.Threading.Tasks;

namespace GIGAClient
{
    class Program
    {
        private static CommandExecutor commands;

        static async Task Main(string[] args)
        {
            commands = new CommandExecutor(false);

            Console.WriteLine($"{args.Length}");
            
            //When client is initiated with a PuppetMaster command
            if (args.Length == 3)
            {
                Console.WriteLine($"Client created with arguments: {args[0]} {args[1]} {args[2]}");
                ClientLogic cl = new ClientLogic(args);

                cl.StartSevice();

                cl.readCommands();
            }

            Console.ReadLine();
        }
    }
}
