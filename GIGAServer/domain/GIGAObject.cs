using System;

namespace GIGAServer.domain
{
    internal class GIGAObject
    {
        private string _value;

        public GIGAObject(GIGAPartitionObject partition, string name, string value, int timestamp)
        {
            Partition = partition ?? throw new ArgumentNullException(nameof(partition));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Locked = false;
            Timestamp = timestamp;
        }

        public GIGAPartitionObject Partition { get; }
        public string Name { get; }

        public string Value
        {
            get => _value;
            set
            {
                if (!Locked) _value = value;
            }
        }

        public bool Locked { get; set; }
        public int Timestamp { get; set; }
    }
}