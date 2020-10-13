﻿using Client.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Scripts.Commands
{
    class WriteCommand : ICommand
    {
        public string Name => "write";

        public string Syntax => "partition_id object_id value";

        public string Description => "Writes the object identified by the <partition_id, object_id> pair and assigns it the quotes delimited string value";

        public int NumArgs => 3;

        void ICommand.SafeExecute(params string[] Args)
        {
            Console.WriteLine(Args.Length);
        }
    }
}
