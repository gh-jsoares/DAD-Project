using GIGAClient.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GIGAClient.Scripts.Commands
{
    class EndRepeatCommand : ICommand
    {
        public string Name => "end-repeat";

        public string Syntax => "";

        public string Description => "Closes a repeat loop.";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, ClientLogic client)
        {
            int currentLoop = 0;
            while (CommandExecutor.LoopCount > 0)
            {
                foreach (KeyValuePair<ICommand, string[]> entry in CommandExecutor.LoopCommands)
                {
                    string[] args = entry.Value.Select(arg => arg.Replace("$i", currentLoop.ToString())).ToArray();
                    entry.Key.Execute(args, client);
                }

                CommandExecutor.LoopCount--;
                currentLoop++;
            }

            CommandExecutor.LoopCommands.Clear();
        }
    }
}
