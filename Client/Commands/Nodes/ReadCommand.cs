﻿using Client.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Scripts.Commands
{
    class ReadCommand : ICommand
    {
        public string Name => "read";

        public string Syntax => "partition_id object_id server_id";

        public string Description => "Reads the object identified by the <partition_id, object_id> pair and outputs the corresponding value. Returns \"N/A\" if the object is not present in the storage system. If server_id is -1 or the client does not need to change server to obtain the requested object, the parameter is ignored.";

        public int NumArgs => 3;

        void ICommand.SafeExecute(params string[] Args)
        {
            Console.WriteLine(Args.Length);
        }
    }
}
