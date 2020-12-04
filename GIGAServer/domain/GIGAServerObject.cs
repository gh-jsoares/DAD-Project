using System;

namespace GIGAServer.domain
{
    internal class GIGAServerObject
    {
        public GIGAServerObject(string name, string url)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public string Name { get; }
        public string Url { get; }

        public override string ToString()
        {
            return string.Format("\"{0}\" @ {1}", Name, Url);
        }
    }
}