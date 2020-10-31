using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    interface ICommand
    {
        string Name { get; }
        string Syntax { get; }
        string Description { get; }
        int NumArgs { get; }

        public String Execute( string[] Args, PuppetMasterLogic PuppetMaster)
        {
            ValidadeArgs(Args);
            SafeExecute(Args, PuppetMaster);
            return $"{Name} Command sent successfully";
        }

        protected void ValidadeArgs(string[] Args)
        {
            if (Args.Length < NumArgs)
            {
                // throw custom exception
                throw new Exception(string.Format("Missing arguments. Syntax: {0} {1}", Name, Syntax));
            }
        }

        protected void SafeExecute(string[] Args, PuppetMasterLogic PuppetMaster);
    }

    /*interface ICommand<T> : ICommand
    {
        public new T Execute(string[] Args)
        {
            ValidadeArgs(Args);
            return SafeExecute(Args);
        }

        protected new T SafeExecute(params string[] Args);
    }*/
}
