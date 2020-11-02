using GIGAClient.Commands;
using System;
using System.Collections.Generic;
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
            while (CommandExecutor.LoopCount > 0)
            {
                foreach (KeyValuePair<ICommand, string[]> entry in CommandExecutor.LoopCommands)
                    entry.Key.Execute(entry.Value, client);

                CommandExecutor.LoopCount--;
            }

            CommandExecutor.LoopCommands.Clear();
        }
    }
}
