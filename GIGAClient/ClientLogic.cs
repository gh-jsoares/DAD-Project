using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using GIGAClient.Commands;
using GIGAClient.services;
using Grpc.Core;

namespace GIGAClient
{
    class ClientLogic
    {
        private string username;
        private string url;
        private string file;

        private Dictionary<string, string> serverMap = new Dictionary<string, string>();
        private Dictionary<string, string[]> partitionMap = new Dictionary<string, string[]>();

        private CommandExecutor commandExecutor = new CommandExecutor();

        public ClientLogic(string[] args)
        {
            username = args[0];
            url = args[1];
            file = args[2];
        }

        internal void StartSevice()
        {

            Uri uri = new Uri(url);

            Server server = new Server                              
            {
                Services = { GIGAPuppetMasterProto.GIGAPuppetMasterService.BindService(new GIGAPuppetMasterService(this)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };

            server.Start();
            Console.WriteLine("Client \"Puppet Master Server\" listening on url {0}", uri.AbsoluteUri);
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
