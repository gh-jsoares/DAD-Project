﻿using ProcessCreationService.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ProcessCreationService.Scripts.Commands
{
    class ServerCommand : ICommand
    {
        public string Name => "Server";

        public string Syntax => "server_id URL min_delay max_delay";

        public string Description => "This command creates a server process identified by server_id, available at URL that delays any incoming message for a random amount of time(specified in milliseconds) between min_delay and max_delay";

        public int NumArgs => 4;

        void ICommand.SafeExecute(params string[] Args)
        {
            Console.WriteLine(Args.Length);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\..\Client\bin\Debug\netcoreapp3.1\Server.exe";
            startInfo.Arguments = @$"{Args[0]} {Args[1]} {Args[2]}";
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(startInfo);
        }
    }
}
