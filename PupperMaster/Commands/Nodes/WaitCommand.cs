﻿using PuppetMaster.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Scripts.Commands
{
    class WaitCommand : ICommand
    {
        public string Name => "Wait";

        public string Syntax => "x_ms";

        public string Description => "Instructs the PuppetMaster to sleep for x_ms milliseconds before reading and executing the next command in the script file";

        public int NumArgs => 1;

        void ICommand.SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster)
        {
            Console.WriteLine(Args.Length);


        }
    }
}
