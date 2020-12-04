namespace GIGAServer.domain
{
    internal class GIGAPartitionObjectID
    {
        public GIGAPartitionObjectID(GIGAObject partitionObject)
        {
            PartitionName = partitionObject.Partition.Name;
            ObjectName = partitionObject.Name;
            MasterServerName = partitionObject.Partition.MasterServer.Name;
        }

        public string PartitionName { get; }
        public string ObjectName { get; }
        public string MasterServerName { get; }
    }
}