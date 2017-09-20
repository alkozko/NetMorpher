namespace NetMorpher.Structures.TrieMap
{
    public struct TrieSearchResult<TRecord>
    {
        public TrieSearchResult(string prefix, TRecord record)
        {
            Prefix = prefix;
            Record = record;
        }

        public string Prefix { get; set; }
        public TRecord Record { get; set; }
    }
}