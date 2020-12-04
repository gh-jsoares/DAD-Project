using System;
using System.Diagnostics;
using GIGAPuppetMaster.domain;
using PuppetMaster.Commands;

namespace PuppetMaster.Scripts.Commands
{
    internal class ServerCommand : ICommand
    {
        public string Name => "Server";

        public string Syntax => "server_id URL min_delay max_delay";

        public string Description =>
            "This command creates a server process identified by server_id, available at URL that delays any incoming message for a random amount of time(specified in milliseconds) between min_delay and max_delay";

        public int NumArgs => 4;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            //Start new server process
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\..\GIGAServer\bin\Debug\netcoreapp3.1\GIGAServer.exe";

            foreach (var arg in Args) startInfo.Arguments += $"{arg} ";

            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(startInfo);

            // TODO: validate args 2 and 3 are ints
            var serverObject = new GIGAServerObject(Args[0], Args[1], int.Parse(Args[2]), int.Parse(Args[3]));
            PuppetMaster.AddServer(serverObject);
        }
    }
}