using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Commands
{
    interface ICommand
    {
        string Name { get; }
        string Syntax { get; }
        string Description { get; }
        int NumArgs { get; }

        public void Execute(params string[] Args)
        {
            ValidadeArgs(Args);
            SafeExecute(Args);
        }

        protected void ValidadeArgs(string[] Args)
        {
            if (Args.Length != NumArgs)
            {
                // throw custom exception
                throw new Exception(string.Format("Missing arguments. Syntax: {0} {1}", Name, Syntax));
            }
        }

        protected void SafeExecute(params string[] Args);
    }

    interface ICommand<T> : ICommand
    {
        public new T Execute(params string[] Args)
        {
            ValidadeArgs(Args);
            return SafeExecute(Args);
        }

        protected new T SafeExecute(params string[] Args);
    }
}
