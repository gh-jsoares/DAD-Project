using GIGAClient.Scripts.Commands;
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
        public static Dictionary<ICommand, string[]> LoopCommands;

        public CommandExecutor()
        {
            this.commands = new HashSet<ICommand>();

            commands.Add(new WaitCommand());
            
        }

        public void Run(string Input, ClientLogic client)
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
                    Command.Execute(ArgsList.ToArray(), client);
                else
                    LoopCommands.Add(Command, ArgsList.ToArray());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
