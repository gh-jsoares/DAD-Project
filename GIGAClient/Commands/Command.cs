using System;
using GIGAClient.services;

namespace GIGAClient.Commands
{
    internal interface ICommand
    {
        string Name { get; }
        string Syntax { get; }
        string Description { get; }
        int NumArgs { get; }


        public void Execute(string[] Args, GIGAClientService service)
        {
            ValidadeArgs(Args);
            SafeExecute(Args, service);
        }

        protected void ValidadeArgs(string[] Args)
        {
            if (Args.Length < NumArgs)
                // throw custom exception
                throw new Exception(string.Format("Missing arguments. Syntax: {0} {1}", Name, Syntax));
        }

        protected void SafeExecute(string[] Args, GIGAClientService service);
    }

    /* interface ICommand<T> : ICommand
     {
         public new T Execute(string[] Args, ClientLogic client))
         {
             ValidadeArgs(Args);
             return SafeExecute(Args);
         }
 
         protected new T SafeExecute(string[] Args, ClientLogic client));
     }*/
}