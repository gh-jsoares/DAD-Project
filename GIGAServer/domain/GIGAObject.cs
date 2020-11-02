using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAServer.domain
{
    class GIGAObject
    {
        public GIGAPartitionObject Partition { get; }
        public string Name { get; }
        public string Value { get => Value; set { if (!Locked) Value = value; } }
        public bool Locked { get; set; }

        public GIGAObject(GIGAPartitionObject partition, string name, string value)
        {
            Partition = partition ?? throw new ArgumentNullException(nameof(partition));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
