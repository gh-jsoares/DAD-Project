using Client.Commands;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Client.Scripts.Commands
{
    class ServerCommand : ICommand
    {
        public string Name => "Server";

        public string Syntax => "server_id URL min_delay max_delay";

        public string Description => "This command creates a server process identified by server_id, available at URL that delays any incoming message for a random amount of time(specified in milliseconds) between min_delay and max_delay";

        public int NumArgs => 4;

        void ICommand.SafeExecute(string[] Args, ClientLogic client)
        {

            Console.WriteLine($"Added new server {Args[0]} with URL {Args[1]}");

            client.ServerMap.Add(Args[0], Args[1]);

        }
    }
}
