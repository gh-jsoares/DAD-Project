using System;
using System.Diagnostics;
using PuppetMaster.Commands;

namespace PuppetMaster.Scripts.Commands
{
    internal class ClientCommand : ICommand
    {
        public string Name => "Client";

        public string Syntax => "username client_URL script_file";

        public string Description =>
            "This command creates a client process identified by the string username, available at client_URL and that will execute the commands in the script file script_file";

        public int NumArgs => 3;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);

            //Create process with given arguments
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = @"..\..\..\..\GIGAClient\bin\Debug\netcoreapp3.1\GIGAClient.exe";
            foreach (var arg in Args) startInfo.Arguments += $"{arg} ";
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(startInfo);


            PuppetMaster.AddClient(Args[0], Args[1]);
        }
    }
}