using Client.Scripts.Commands;
using Microsoft.Extensions.Logging;
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

        public CommandExecutor(bool PuppetMasterCommands)
        {
            this.commands = new HashSet<ICommand>();

            //Commands sent by the Puppet Master to the client
            if(PuppetMasterCommands)
            {
                commands.Add(new PartitionCommand());
                commands.Add(new ServerCommand());
                commands.Add(new StatusCommand());
            }

            //Commands run by the client's script (write, read, list_server, etc.)
            else    
            {
                commands.Add(new WaitCommand());
            }

           
            
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
                Command.Execute(ArgsList.ToArray(), client);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
