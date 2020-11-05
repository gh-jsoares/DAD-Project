using System;

namespace GIGAPuppetMaster.domain
{
    class GIGAServerObject
    {
        public string Name { get; }
        public string Url { get; }
        public int MinDelay { get; }
        public int MaxDelay { get; }

        public GIGAServerObject(string name, string url, int minDelay, int maxDelay)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            MinDelay = minDelay;
            MaxDelay = maxDelay;
        }
    }
}
