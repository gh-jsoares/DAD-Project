using GIGAClient.Commands;
using GIGAClient.services;

namespace GIGAClient.Scripts.Commands
{
    internal class WaitCommand : ICommand
    {
        public string Name => "wait";

        public string Syntax => "x";

        public string Description => "Delays the execution of the next command for x milliseconds.";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, GIGAClientService service)
        {
            // TODO VALIDATE INT
            var amount = int.Parse(Args[0]);
            GIGAClientService.Wait(amount);
        }
    }
}