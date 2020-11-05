using System;

namespace GIGAPuppetMaster.domain
{
    class GIGAServerObject
    {
        public string Name { get; }
        public string Url { get; }

        public GIGAServerObject(string name, string url)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}
