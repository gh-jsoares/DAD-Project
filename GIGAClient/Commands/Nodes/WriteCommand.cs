using GIGAClient.Commands;
using GIGAClient.services;

namespace GIGAClient.Scripts.Commands
{
    internal class WriteCommand : ICommand
    {
        public string Name => "write";

        public string Syntax => "partition_id object_id value";

        public string Description =>
            "Writes the object identified by the <partition_id, object_id> pair and assigns it the quotes delimited string value";

        public int NumArgs => 3;

        void ICommand.SafeExecute(string[] Args, GIGAClientService service)
        {
            var partitionId = Args[0];
            var objectId = Args[1];
            var value = Args[2];

            service.Write(partitionId, objectId, value);
        }
    }
}