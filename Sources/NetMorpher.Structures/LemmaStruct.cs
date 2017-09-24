using System;
using System.Linq;

namespace NetMorpher.Structures
{
    [Serializable]
    public class LemmaStruct
    {
        public int Id { get; }
        public string Lemma { get; }
        public byte[] Gr { get; }
        public int Count { get; }
        public LemmaStruct Parent { get; set; }

        public LemmaStruct(int id, string lemma, byte[] gr, int count)
        {
            Id = id;
            Lemma = lemma;
            Gr = gr;
            Count = count;
        }

        public LemmaStruct GetFinal()
        {
            if (Parent != null)
                return Parent.GetFinal();

            return this;
        }

        public ILemmaInfo CreateInfo(string[] grIndex)
        {
            return new LemmaInfo(Lemma, Gr.Select(x => grIndex[x]).ToArray());
        }
    }

    internal class LemmaInfo : ILemmaInfo
    {
        public LemmaInfo(string lemma, string[] gr)
        {
            Gr = gr;
            Lemma = lemma;
        }

        public string Lemma { get; }
        public string[] Gr { get; }
    }

    public interface ILemmaInfo
    {
        string Lemma { get; }
        string[] Gr { get; }
    }
}