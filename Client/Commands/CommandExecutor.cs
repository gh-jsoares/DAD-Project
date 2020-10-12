using Client.Scripts.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Client.Commands
{
    class CommandExecutor
    {
        private HashSet<ICommand> commands;

        public CommandExecutor()
        {
            this.commands = new HashSet<ICommand>();
            commands.Add(new WaitCommand());
        }

        public void Run(string Input)
        {
            string[] SplitInput = Regex.Replace(Input.Trim(), @"\s+", " ").Split(" ");

            if (SplitInput.Length == 0)
            {
                // TODO: use logger
                Console.WriteLine("No command specified");
                return;
            }

            string Name = SplitInput[0];
            List<string> ArgsList = new List<string>(SplitInput);
            ArgsList.RemoveAt(0);

            ICommand Command = commands.Where((c) => c.Name == Name).FirstOrDefault();
            if (Command == null)
            {
                // TODO: use logger
                Console.WriteLine("Command not found");
                return;
            }

            try
            {
                Command.Execute(ArgsList.ToArray());
            }
            catch (Exception e)
            {
                // TODO: use logger
                Console.WriteLine(e.Message);
            }
        }
    }
}
