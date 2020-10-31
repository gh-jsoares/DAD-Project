using Client.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Scripts.Commands
{
    class ListServerCommand : ICommand
    {
        public string Name => "listServer";

        public string Syntax => "server_id";

        public string Description => "Lists all objects stored on the server identified by _server_id and whether the serveris the master replica for that object or not";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, ClientLogic client)
        {
            throw new NotImplementedException();
        }
    }
}
