using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMorpher.Structures.TrieMap
{
    /// <summary>
    /// The Trie Map Data Structure (a.k.a Prefix Tree).
    /// </summary>
    /// <typeparam name="TRecord">The type of records attached to words</typeparam>
    [Serializable]
    public class TrieMap<TRecord> where TRecord: class 
    {
        private int _count;
        private readonly TrieMapNode<TRecord> _root;

        public TrieMap()
        {
            _count = 0;
            _root = new TrieMapNode<TRecord>(' ', default(TRecord));
        }

        public TrieMap(IEnumerable<TrieMapNode<TRecord>> childs)
        {
            _count = 0;
            _root = new TrieMapNode<TRecord>(' ', default(TRecord));
            foreach (var child in childs)
            {
                _root.AddChildren(child.Key, child);
            }
        }

        public List<TrieMapNode<TRecord>> GetChilds()
        {
            return _root.Children.ToList();
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public void Add(string word, TRecord record)
        {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentException("Word is empty or null.");

            var current = _root;

            foreach (char key in word)
            {
                var child = current.GetChildrenOrDefault(key);

                if (child == null)
                {
                    var newTrieNode = new TrieMapNode<TRecord>(key, default(TRecord));
                    child = current.AddChildren(key, newTrieNode);
                }

                current = child;
            }

            if (current.Record != null)
                throw new ApplicationException("Word already exists in Trie.");

            ++_count;
            current.Record = record;
        }

        private bool FuzzySearchByWord(string word, out TrieSearchResult<TRecord>[] records, out string prefix)
        {
            if (string.IsNullOrEmpty(word))
                throw new ApplicationException("Word is either null or empty.");

            var prefixWord = new StringBuilder();

            var current = _root;

            foreach (char key in word)
            {
                var child = current.GetChildrenOrDefault(key);

                if (child == null)
                {
                    records = GetAllChilds(current, prefixWord.ToString());
                    prefix = prefixWord.ToString();
                    return false;
                }

                prefixWord.Append(key);
                current = child;
            }

            if (current.Record == null)
            {
                records = GetAllChilds(current, prefixWord.ToString());
                prefix = prefixWord.ToString();
                return false;
            }

            records = new[] {new TrieSearchResult<TRecord>(prefixWord.ToString(), current.Record) };
            prefix = prefixWord.ToString();
            return true;
        }

        private TrieSearchResult<TRecord>[] GetAllChilds(TrieMapNode<TRecord> current, string prefixWord)
        {
            var list = new List<TrieSearchResult<TRecord>>();

            if (current.Record != null)
            {
                list.Add(new TrieSearchResult<TRecord>(prefixWord, current.Record));
            }

            foreach (var currentChild in current.Children)
            {
                var newPrefixWord = prefixWord + currentChild.Key;
                //if (currentChild.Record != null)
                    //list.Add(new TrieSearchResult<TRecord>(newPrefixWord, currentChild.Record));
                //else
                {
                    list.AddRange(GetAllChilds(currentChild, newPrefixWord));
                }
            }
            return list.ToArray();
        }

        public TrieSearchResultSet<TRecord> FuzzySearch(string token)
        {
            TrieSearchResult<TRecord>[] records;
            string prefix;

            bool found = FuzzySearchByWord(token, out records, out prefix);
            return new TrieSearchResultSet<TRecord>
            {
                Found = found,
                Prefix = prefix,
                Records = records,
            };
        }
    }
}