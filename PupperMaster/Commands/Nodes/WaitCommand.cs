using System.Threading;
using PuppetMaster.Commands;

namespace PuppetMaster.Scripts.Commands
{
    internal class WaitCommand : ICommand
    {
        public string Name => "Wait";

        public string Syntax => "x_ms";

        public string Description =>
            "Instructs the PuppetMaster to sleep for x_ms milliseconds before reading and executing the next command in the script file";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            // TODO VALIDATE INT
            var amount = int.Parse(Args[0]);
            Thread.Sleep(amount);
        }
    }
}