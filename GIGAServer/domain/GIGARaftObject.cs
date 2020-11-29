using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GIGAServer.domain
{
    class GIGARaftObject
    {
        public int State { get; set; }
        public int Term { get; set; }
        public int Timeout { get; set; }
        public int Votes { get; set; }

        public GIGARaftObject()
        {
            State = 1;
            Term = 0;

            Random rnd = new Random();
            Timeout = rnd.Next(1000, 2000);

            Votes = 0;
        }

       
    }
}
