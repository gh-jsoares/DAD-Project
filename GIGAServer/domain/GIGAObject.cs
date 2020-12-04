﻿using System;

namespace GIGAServer.domain
{
    class GIGAObject
    {
        public GIGAPartitionObject Partition { get; }
        public string Name { get; }
        private string _value;
        public string Value { get => _value; set { if (!Locked) _value = value; } }
        public bool Locked { get; set; }
        public int Timestamp { get; set; }

        public GIGAObject(GIGAPartitionObject partition, string name, string value, int timestamp)
        {
            Partition = partition ?? throw new ArgumentNullException(nameof(partition));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Locked = false;
            Timestamp = timestamp;
        }
    }
}
