using PuppetMaster.Scripts.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PuppetMaster.Commands
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

        public string Run(string Input, PuppetMasterLogic puppetMaster)
        {
            string[] SplitInput = Regex.Replace(Input.Trim(), @"\s+", " ").Split(" ");

            if (SplitInput.Length == 0)
            {
                Console.Error.WriteLine("No command specified");
                return "No command specified";
            }

            string Name = SplitInput[0];
            List<string> ArgsList = new List<string>(SplitInput);
            ArgsList.RemoveAt(0);

            ICommand Command = commands.Where((c) => c.Name == Name).FirstOrDefault();
            if (Command == null)
            {
                Console.Error.WriteLine("No command found for {0}", Name);
                return  $"No command found for {Name}";
            }

            try
            {
                return Command.Execute(ArgsList.ToArray(), puppetMaster);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);

                return e.Message;
            }
        }
    }
}
