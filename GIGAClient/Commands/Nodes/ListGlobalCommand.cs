using GIGAClient.Commands;
using GIGAClient.services;

namespace GIGAClient.Scripts.Commands
{
    internal class ListGlobalCommand : ICommand
    {
        public string Name => "listGlobal";

        public string Syntax => "";

        public string Description => "Lists the partition and object identifiers of all objects stored on the system.";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, GIGAClientService service)
        {
            service.ListGlobal();
        }
    }
}