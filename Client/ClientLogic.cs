using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Client.Commands;
using Grpc.Core;
using PuppetMaster;

namespace Client
{
    class ClientLogic
    {
        private string username;
        private string url;
        private string file;

        private Dictionary<string, string> serverMap = new Dictionary<string, string>();
        private Dictionary<string, string[]> partitionMap = new Dictionary<string, string[]>();

        private CommandExecutor commandExecutor = new CommandExecutor(false);

        

        public ClientLogic(string[] args)
        {
            username = args[0];
            url = args[1];
            file = args[2];
        }

        internal void StartSevice()
        {

            Uri uri = new Uri(url);

            Grpc.Core.Server server = new Grpc.Core.Server                              //Change Server project namespace
            {
                Services = { CommandListener.BindService(new ClientService(this)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.WriteLine($"Client server listening on host {uri.Host} and port {uri.Port} ");
        }

        internal void readCommands()
        {

            string command;
            
            while (true)
            {
                Console.WriteLine("Insert a new command:");

                command = Console.ReadLine();

                Console.WriteLine($"Running command {command}");

                commandExecutor.Run(command, this);
            }
        }


        public string Username { get => username; set => username = value; }
        public string Url { get => url; set => url = value; }
        public Dictionary<string, string> ServerMap { get => serverMap; set => serverMap = value; }
        public Dictionary<string, string[]> PartitionMap { get => partitionMap; set => partitionMap = value; }
    }
}
