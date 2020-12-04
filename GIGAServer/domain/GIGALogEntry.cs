namespace GIGAServer.domain
{
    internal class GIGALogEntry
    {
        public GIGALogEntry(int term, int index, GIGAObject data)
        {
            Index = index;
            Term = term;
            Data = data;
            Committed = false;
        }

        public int Term { get; set; }
        public int Index { get; set; }
        public GIGAObject Data { get; set; }
        public bool Committed { get; set; }
    }
}