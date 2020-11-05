using GIGAClient.Scripts.Commands;
using GIGAClient.services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GIGAClient.Commands
{
    class CommandExecutor
    {
        private HashSet<ICommand> commands;
        public static int LoopCount = 0;
        public static Dictionary<ICommand, string[]> LoopCommands = new Dictionary<ICommand, string[]>();
        private readonly GIGAClientService gigaClientService;

        public CommandExecutor(services.GIGAClientService gigaClientService)
        {
            this.commands = new HashSet<ICommand>();
 
            commands.Add(new ListGlobalCommand());
            commands.Add(new ReadCommand());
            commands.Add(new WriteCommand());
            commands.Add(new ListServerCommand());
            commands.Add(new WaitCommand());
            commands.Add(new BeginRepeatCommand());
            commands.Add(new EndRepeatCommand());

            this.gigaClientService = gigaClientService;
         
        }

        public void Run(string Input)
        {
            string[] SplitInput = Regex.Replace(Input.Trim(), @"\s+", " ").Split(" ");

            if (SplitInput.Length == 0)
            {
                Console.Error.WriteLine("No command specified");
                return;
            }

            string Name = SplitInput[0];
            List<string> ArgsList = new List<string>(SplitInput);
            ArgsList.RemoveAt(0);

            ICommand Command = commands.Where((c) => c.Name == Name).FirstOrDefault();
            if (Command == null)
            {
                Console.Error.WriteLine("No command found for {0}", Name);
                return;
            }

            try
            {
                if (LoopCount == 0 || Command is EndRepeatCommand)
                    Command.Execute(ArgsList.ToArray(), gigaClientService);
                else
                    LoopCommands.Add(Command, ArgsList.ToArray());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            }
        }
    }
}
