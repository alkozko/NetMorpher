using System;
using System.Collections.Generic;
using System.Linq;

namespace NetMorpher.Structures.TrieMap
{
    [Serializable]
    public class TrieMapNode<TRecord>: IComparable<TrieMapNode<TRecord>>
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

        public TrieMapNode<TRecord>[] Children => _children ?? new TrieMapNode<TRecord>[0];

        public TrieMapNode<TRecord> GetChildrenOrDefault(char ch)
        {
            if (_children == null)
                return null;

            var binarySearchResult = Array.BinarySearch(_children, new TrieMapNode<TRecord>(ch, default(TRecord)));
            if (binarySearchResult < 0)
                return null;

            return _children[binarySearchResult];
        }

        public TrieMapNode<TRecord> AddChildren(char ch, TrieMapNode<TRecord> newTrieNode)
        {
            if (_children == null)
                _children = new TrieMapNode<TRecord>[0];

            _children = _children.Concat(new [] {newTrieNode}).OrderBy(x => x.Key).ToArray();
            return newTrieNode;
        }

        public int CompareTo(TrieMapNode<TRecord> other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Key.CompareTo(other.Key);
        }
    }
}