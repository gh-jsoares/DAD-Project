using System;
using System.Collections.Generic;
using System.Text;

namespace GIGAServer.domain
{
    class GIGALogEntry
    {
        public int Term { get; set; }
        public int Index { get; set; }
        public GIGAObject Data { get; set; }
        public bool Committed { get; set; }

        public GIGALogEntry(int term, int index, GIGAObject data)
        {
            Index = index;
            Term = term;
            Data = data;
            Committed = false;

        }
    }

  
}
