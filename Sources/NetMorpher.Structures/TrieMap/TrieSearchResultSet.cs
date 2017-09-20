namespace NetMorpher.Structures.TrieMap
{
    public struct TrieSearchResultSet<TRecord>
    {
        public bool Found { get; set; }
        public string Prefix { get; set; }
        public TrieSearchResult<TRecord>[] Records { get; set; }
    }
}