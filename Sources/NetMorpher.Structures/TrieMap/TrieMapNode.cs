using System.Collections.Generic;
using System.Linq;

namespace NetMorpher.Structures.TrieMap
{
    internal class TrieMapNode<TRecord>
    {
        public char Key;

        public TRecord Record;

        private TrieMapNode<TRecord>[] _children;


        public TrieMapNode(char key, TRecord record)
        {
            Key = key;
            Record = record;
            _children = null;
        }

        public IEnumerable<TrieMapNode<TRecord>> Children => _children ?? new TrieMapNode<TRecord>[0];

        public TrieMapNode<TRecord> GetChildren(char ch)
        {
            return _children.Single(x => x.Key == ch);
        }

        public bool HasChildren(char ch)
        {
            return _children != null && _children.Any(x => x.Key == ch);
        }

        public void AddChildren(char ch, TrieMapNode<TRecord> newTrieNode)
        {
            if (_children == null)
                _children = new TrieMapNode<TRecord>[0];

            _children = _children.Concat(new [] {newTrieNode}).ToArray();
        }
    }
}