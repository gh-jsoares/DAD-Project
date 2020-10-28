using ProcessCreationService.Scripts.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessCreationService.Commands
{
    class CommandExecutor
    {
        private HashSet<ICommand> commands;

        public CommandExecutor()
        {
            this.commands = new HashSet<ICommand>();
            commands.Add(new ClientCommand());
            commands.Add(new CrashCommand());
            commands.Add(new FreezeCommand());
            commands.Add(new PartitionCommand());
            commands.Add(new ReplicationFactorCommand());
            commands.Add(new ServerCommand());
            commands.Add(new StatusCommand());
            commands.Add(new UnfreezeCommand());
            commands.Add(new WaitCommand());
        }

        public CommandReply Run(string Input)
        {
            string[] SplitInput = Regex.Replace(Input.Trim(), @"\s+", " ").Split(" ");

            CommandReply cr = new CommandReply { };

            if (SplitInput.Length == 0)
            {
                Console.Error.WriteLine("No command specified");
                cr.Ok = false;
                cr.Error = "No command specified";
                return cr;
            }

            string Name = SplitInput[0];
            List<string> ArgsList = new List<string>(SplitInput);
            ArgsList.RemoveAt(0);

            ICommand Command = commands.Where((c) => c.Name == Name).FirstOrDefault();
            if (Command == null)
            {
                Console.Error.WriteLine("No command found for {0}", Name);
                cr.Ok = false;
                cr.Error = $"No command found for {Name}";
                return cr;
            }

            try
            {
                cr = Command.Execute(ArgsList.ToArray());

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);

                cr.Ok = false;
                cr.Error = e.Message;
                return cr;
            }

            return cr;
        }
    }
}
