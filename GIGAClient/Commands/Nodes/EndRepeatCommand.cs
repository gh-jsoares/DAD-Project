using System.Linq;
using GIGAClient.Commands;
using GIGAClient.services;

namespace GIGAClient.Scripts.Commands
{
    internal class EndRepeatCommand : ICommand
    {
        public string Name => "end-repeat";

        public string Syntax => "";

        public string Description => "Closes a repeat loop.";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, GIGAClientService service)
        {
            var currentLoop = 1;
            while (CommandExecutor.LoopCount > 0)
            {
                foreach (var entry in CommandExecutor.LoopCommands)
                {
                    var args = entry.Value.Select(arg => arg.Replace("$i", currentLoop.ToString())).ToArray();
                    entry.Key.Execute(args, service);
                }

                CommandExecutor.LoopCount--;
                currentLoop++;
            }

            CommandExecutor.LoopCommands.Clear();
        }
    }
}