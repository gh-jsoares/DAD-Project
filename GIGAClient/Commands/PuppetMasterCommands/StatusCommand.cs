using GIGAClient.Commands;
using System;

namespace GIGAClient.Scripts.Commands
{
    class StatusCommand : ICommand
    {
        public string Name => "Status";

        public string Syntax => "";

        public string Description => "Makes all nodes in the system print their current status";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, ClientLogic client)
        {
            Console.WriteLine($"Client {client.Username} is up in URL {client.Url}");

        }
    }
}
