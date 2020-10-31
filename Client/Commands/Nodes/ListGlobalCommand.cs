using Client.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Scripts.Commands
{
    class ListGlobalCommand : ICommand
    {
        public string Name => "listGlobal";

        public string Syntax => "";

        public string Description => "Lists the partition and object identifiers of all objects stored on the system.";

        public int NumArgs => 0;

        void ICommand.SafeExecute(string[] Args, ClientLogic client)
        {
            throw new NotImplementedException();
        }
    }
}
