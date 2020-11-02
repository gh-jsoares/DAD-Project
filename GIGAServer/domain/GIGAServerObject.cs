using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAServer.domain
{
    class GIGAServerObject
    {
        public string Name { get; }

        public GIGAServerObject(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
