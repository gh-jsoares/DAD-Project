using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAServer.domain
{
    class GIGAPartitionObjectID
    {
        public string PartitionName { get; }
        public string ObjectName { get; }

        public GIGAPartitionObjectID(GIGAObject partitionObject)
        {
            PartitionName = partitionObject.Partition.Name;
            ObjectName = partitionObject.Name;
        }
    }
}
